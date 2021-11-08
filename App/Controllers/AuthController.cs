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
    }
}
