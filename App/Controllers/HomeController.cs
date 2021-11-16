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
            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            try {
                ViewBag.CurUserId = curUser.Id;
            } catch {
                ViewBag.CurUserId = null;
            }

            var questions = await _db.Questions.ToListAsync();
            return View(questions);
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
