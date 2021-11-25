using App.Models;
using App.Context;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    [Authorize(Roles = "Admin")]
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
                var questions = await _db.Questions.Where(p => p.UserId == id).ToListAsync();
                foreach (var question in questions) {
                    var qcomments = _db.Comments.Where(p => p.QuestionId == question.Id).ToList();
                    foreach (var comment in qcomments)
                        _db.Comments.Remove(comment);
                    var answers = await _db.Answers.Where(p => p.QuestionId == question.Id).ToListAsync();
                    foreach (var answer in answers) {
                        var comments = await _db.Comments.Where(p => p.AnswerId == answer.Id).ToListAsync();
                        foreach (var comment in comments) {
                            _db.Comments.Remove(comment);
                        }
                        _db.Answers.Remove(answer);
                    }
                    _db.Questions.Remove(question);
                }

                // Clean comments
                foreach (var comment in user.Comments) _db.Comments.Remove(comment);
                // Clean liked posts
                foreach (var liked in user.LikedPosts) _db.LikedPosts.Remove(liked);
                // Clean answers
                foreach (var answer in user.Answers) _db.Answers.Remove(answer);
                // Clean questions
                foreach (var question in user.Questions) {
                    foreach (var tag in question.Tags)
                        _db.Tags.Remove(tag);
                    _db.Questions.Remove(question);
                }

                // Remove deleted user from roles.
                foreach (var item in rolesForUser.ToList())
                    await _userManager.RemoveFromRoleAsync(user, item);
#endregion

                // If you delete yourself, sign out.
                if (id == curUser.Id)
                    await _signInManager.SignOutAsync();

                await _db.SaveChangesAsync(); // Save changes to the database
                await _userManager.DeleteAsync(user); // Delete user
                await _db.SaveChangesAsync(); // Save changes to the database
                await transaction.CommitAsync(); // Commit the transaction
            }
            return RedirectToAction("UsersList", "Admin"); // Admin/UserList
        }
    }
}
