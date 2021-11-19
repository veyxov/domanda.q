using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using App.Models;
using MoviePortal.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuestionsContext _db;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, QuestionsContext db, UserManager<User> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            try {
                var curUser = await _userManager.GetUserAsync(HttpContext.User);
                var roles = await _userManager.GetRolesAsync(curUser);

                foreach (var i in roles)
                    _logger.Log(LogLevel.Critical, i.ToString());
            } catch (Exception ex) {
                _logger.Log(LogLevel.Warning, ex.Message);
                _logger.Log(LogLevel.Warning, "No user found");
            }

            // Default home page based on the user role
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (User.IsInRole("moderator"))
            {
                return RedirectToAction("Index", "Moderator");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
