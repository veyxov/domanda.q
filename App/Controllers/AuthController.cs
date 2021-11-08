using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /*[HttpPost]
        public async Task<IActionResult> LoginAsync(UserLoginDTO newUser)
        {
        }*/

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

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            return RedirectToAction("Login", "Auth");
        }
    }
}
