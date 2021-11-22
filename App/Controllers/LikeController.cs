using System;
using App.Models;
using App.Context;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace App.Controllers
{
    [Authorize]
    public class LikeController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;
        private readonly UserManager<User> _userManager;

        public LikeController(QuestionsContext db, ILogger<QuestionController> logger, UserManager<User> userManager) {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        /* Adds val to entity.Likes */
        [NonAction]
        public async Task<IActionResult> Change(Guid id, int val)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            // If you already like this post
            if (_db.LikedPosts.Any(p => p.PostId == id && p.UserId == curUser.Id)) {
                _logger.Log(LogLevel.Critical, "You already liked/disliked this post");
                val = 0;
            }
            else
            {
                // Like and add to database
                curUser.LikedPosts.Add( new LikedPost() { PostId = id, UserId = curUser.Id });
            }

            // Find out that the like belongs to question or answer
            var question = await _db.Questions.FindAsync(id);
            // ANSWER
            if ( question == null )
            {
                var answer = await _db.Answers.FindAsync(id);
                answer.Likes += val;
                await _db.SaveChangesAsync();
                return RedirectToAction(
                    "Show", "Question", new { Id = answer.QuestionId });
            }
            // QUESTION
            else
            {
                question.Likes += val;
                await _db.SaveChangesAsync();
                return RedirectToAction("Show", "Question", new { Id = id });
            }
        }
        // GET: Like/Increment
        public async Task<IActionResult> IncrementAsync(Guid id, int val) => await Change(id, 1);

        // GET: Like/Decrement
        public async Task<IActionResult> DecrementAsync(Guid id, int val) => await Change(id, -1);
    }
}
