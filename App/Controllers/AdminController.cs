using App.Models;
using App.Context;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuestionsContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AdminController(
            ILogger<HomeController> logger,
            QuestionsContext db,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Admin/
        public IActionResult Index() => View();

        // GET: Admin/UserList
        public async Task<IActionResult> UsersListAsync()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        // GET: Admin/DeleteUser
        // ROUTE: id
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            // User associated data
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(id);
            var rolesForUser = await _userManager.GetRolesAsync(user);

            // SQL Transaction
            using (var transaction = _db.Database.BeginTransaction())
            {
#region Cleanup
                // Clean the entityes before removing
                var questions = await _db.Questions.Where(p => p.UserId == user.Id).ToListAsync();
                foreach (var question in questions)
                {
                    var answers = await _db.Answers.Where(p => p.QuestionId == question.Id).ToListAsync();
                    foreach (var answer in answers) {
                        answer.Comments.Clear(); // Answer->Comments
                        _db.Answers.Remove(answer); // Answer
                    }
                    answers.Clear(); // Answers

                    // Remove dependencies
                    question.Comments.Clear(); // Question->Comments
                    question.Answers.Clear();  // Question->Answers
                    _db.Questions.Remove(question); // Question
                    await _db.SaveChangesAsync();
                }
                questions.Clear(); // Questions
#endregion

                // If you delete yourself, sign out.
                if (id == curUser.Id)
                    await _signInManager.SignOutAsync();

                // Remove deleted user from roles.
                foreach (var item in rolesForUser.ToList())
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, item);
                }

                await _db.SaveChangesAsync(); // Save changes to the database
                await _userManager.DeleteAsync(user); // Delete user
                transaction.Commit(); // Commit the transaction
            }
            return RedirectToAction("UsersList", "Admin"); // Admin/UserList
        }
    }
}
