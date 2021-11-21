using System;
using App.Models;
using App.Context;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuestionsContext _db;
        private readonly UserManager<User> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            QuestionsContext db,
            UserManager<User> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        // GET: Home/Index
        public async Task<IActionResult> IndexAsync()
        {
            try {
                var curUser = await _userManager.GetUserAsync(HttpContext.User);
                var roles = await _userManager.GetRolesAsync(curUser);
            } catch (Exception ex) {
                _logger.Log(LogLevel.Warning, ex.Message);
                _logger.Log(LogLevel.Warning, "No user found");
            }

            // Default home page based on the user role
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
            else if (User.IsInRole("Moderator"))
                return RedirectToAction("Index", "Moderator");

            return View();
        }

        // GET: Home/Privacy
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
