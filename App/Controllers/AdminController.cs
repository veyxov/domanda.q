using System;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviePortal.Context;

namespace App.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuestionsContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AdminController(ILogger<HomeController> logger, QuestionsContext db, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> UsersListAsync()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var rolesForUser = await _userManager.GetRolesAsync(user);

            // Clean the entityes before removing
            var questions = await _db.Questions.Where(p => p.UserId == user.Id).ToListAsync();
            foreach (var question in questions)
            {
                var answers = await _db.Answers.Where(p => p.QuestionId == question.Id).ToListAsync();
                foreach (var answer in answers)
                {
                    answer.Comments.Clear();
                }
                // Remove dep
                question.Comments.Clear();
                question.Answers.Clear();
            }
            questions.Clear();
            // -----------
            await _signInManager.SignOutAsync();

            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    var result = await _userManager.RemoveFromRoleAsync(user, item);
                }

                await _db.SaveChangesAsync();
                await _userManager.DeleteAsync(user);
                transaction.Commit();
            }
            return RedirectToAction("UsersList", "Admin");
        }
    }
}
