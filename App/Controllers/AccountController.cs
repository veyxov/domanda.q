using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, SignInManager<User> signInManager, UserManager<User> userManager, IWebHostEnvironment webHostEnv)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnv = webHostEnv;
        }


        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserLoginDTO newUser)
        {
            if (!ModelState.IsValid)
                return View(newUser);

            var result = await _signInManager.PasswordSignInAsync(newUser.Username, newUser.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("LoginFail", "Username or password is wrong.");
                return View(newUser);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserRegisterDTO dto) 
        {
            if (!ModelState.IsValid)
                return View(dto);

            var newUser = new User() {
                UserName = dto.Username,
                Email = dto.Email,
            };

            newUser.ProfilePicPath = await CreateFile(_webHostEnv.WebRootPath, dto.ProfilePicFile);
            
            if (dto.ProfilePicFile is not null)
                _logger.Log(LogLevel.Debug, dto.ProfilePicFile.FileName);

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("LoginFail", "Username or password is wrong.");
                return View(dto);
            }
            
            // If everyting is ok, Login in and redirecto to HOME
            await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> ManageAsync()
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var dto = new UserRegisterDTO()
            {
                Username = curUser.UserName,
                Email = curUser.Email,
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> ManageAsync(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            if (dto.ProfilePicFile is null)
                _logger.Log(LogLevel.Critical, "NOFILE");

            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            // UserName
            curUser.UserName = dto.Username;
            // Email
            curUser.Email = dto.Email;
            // Profile Pic
            curUser.ProfilePicPath = await CreateFile(_webHostEnv.WebRootPath, dto.ProfilePicFile);
            // Password
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(curUser);
            var result = await _userManager.ResetPasswordAsync(curUser, token, dto.Password);

            if (dto.ProfilePicFile is not null)
                _logger.Log(LogLevel.Critical, dto.ProfilePicFile.FileName);

            await _userManager.UpdateAsync(curUser);
            return View(dto);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

#region NonAction 
        // Creates a file with the given file name and current time
        // Returns the path of the saved picture
        [NonAction]
        public async Task<string> CreateFile(string rootPath, IFormFile file)
        {
            if (file == null) return null;

            _logger.Log(LogLevel.Information, rootPath);

            // CURDATE_FileName.Extension
            var fileName = $"{DateTime.Now.ToString("yyMMddHHmmssff")}_{file.FileName}";
            var filePath = Path.Combine(rootPath, "data/images", fileName);
            _logger.Log(LogLevel.Critical, filePath);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/data/images/{fileName}";
        }
#endregion
    }
}
