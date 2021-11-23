using System;
using System.IO;
using App.Models;
using System.Linq;
using App.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            QuestionsContext db,
            ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IWebHostEnvironment webHostEnv)
        {
            _db = db;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnv = webHostEnv;
        }


        // GET: Account
        public IActionResult Index() => View();

        // GET: Account/Login
        public IActionResult Login() => View();

        // POST: Account/Login
        // BODY: UserLoginDTO
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserLoginDTO newUser)
        {
            if (!ModelState.IsValid) return View(newUser);

            var result = await _signInManager.PasswordSignInAsync(
                userName: newUser.Username,
                password: newUser.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(key: "LoginFail", errorMessage: "Username or password is wrong.");
                return View(newUser);
            }

            return RedirectToAction("Index", "Home"); // Home/Index
        }

        // GET: Account/Register
        [AllowAnonymous]
        public IActionResult Register() => View();

        // POST: Account/Register
        // BODY: UserRegisterDTO
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserRegisterDTO dto) 
        {
            if (!ModelState.IsValid) return View(dto);

            var newUser = new User() {
                UserName = dto.Username,
                Email = dto.Email,
            };

            // Write the picture to the file system and get the path
            newUser.ProfilePicPath = await CreateFile(
                _webHostEnv.WebRootPath, dto.ProfilePicFile);

            // Create new User
            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("LoginFail", "Username or password is wrong.");
                return View(dto);
            }
            
            // If everyting is ok, Login and redirecto to HOME/INDEX
            await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
            return RedirectToAction("Index", "Home"); // Home/Index
        }

        // GET: Account/Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account"); // Account/Login
        }

        // GET: Account/Manage
        [Authorize]
        public async Task<IActionResult> ManageAsync()
        {
            // Return current user information to the view
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var dto = new UserRegisterDTO()
            {
                Username = curUser.UserName,
                Email = curUser.Email,
            };

            return View(dto);
        }

        // POST: Account/Manage
        // BODY: UserRegisterDTO
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ManageAsync(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            // UserName
            curUser.UserName = dto.Username;

            // Email
            curUser.Email = dto.Email;
            
            // Profile Pic
            if (dto.ProfilePicFile is not null) {
                curUser.ProfilePicPath = await CreateFile(_webHostEnv.WebRootPath, dto.ProfilePicFile);
            }

            // Password
            var token = await _userManager.GeneratePasswordResetTokenAsync(curUser);
            var result = await _userManager.ResetPasswordAsync(curUser, token, dto.Password);

            await _userManager.UpdateAsync(curUser);
            return View(dto);
        }

        [AllowAnonymous]
        // GET: Account/AccessDenied
        // ROUTE: returnURL
        public IActionResult AccessDenied(string returnUrl = null) => View();

        // GET: Account/ShowProfile
        // ROUTE: id
        public IActionResult ShowProfile(string id)
        {
            var user = _db.Users.Where(p => p.Id == id).FirstOrDefault();
            if (user == null)
                return NotFound("User not found");

            return View(user);
        }

#region NonAction 
        // Creates a file with the given file name and current time
        // Returns the path of the saved picture
        [NonAction]
        public async Task<string> CreateFile(string rootPath, IFormFile file)
        {
            if (file == null) return null;

            // CURDATE_FileName.Extension
            var fileName = $"{DateTime.Now.ToString("yyMMddHHmmssff")}_{file.FileName}";
            var filePath = Path.Combine(rootPath, "data/images", fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/data/images/{fileName}";
        }
#endregion
    }
}
