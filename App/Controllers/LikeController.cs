using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoviePortal.Context;

namespace App.Controllers
{
    public class LikeController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;

        public LikeController(QuestionsContext db, ILogger<QuestionController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> IncrementAsync(Guid id)
        {
            try {
                var question = await _db.Questions.FindAsync(id);
                // Increment the likes
                question.Likes += 1;
                await _db.SaveChangesAsync();
                return RedirectToAction("Show", "Question", new { Id = id });
            } catch {
                var question = await _db.Answers.FindAsync(id);
                // Increment the likes
                question.Likes += 1;
                return RedirectToAction("Show", "Question", new { Id = question.QuestionId });
            } finally {
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> DecrementAsync(Guid id)
        {
            try {
                var question = await _db.Questions.FindAsync(id);
                // Increment the likes
                question.Likes -= 1;
                return RedirectToAction("Show", "Question", new { Id = id });
            } catch {
                var question = await _db.Answers.FindAsync(id);
                // Increment the likes
                question.Likes -= 1;
                return RedirectToAction("Show", "Question", new { Id = question.QuestionId });
            }
            finally {
                await _db.SaveChangesAsync();
            }
        }
    }
}
