using System;
using App.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    public class LikeController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;

        public LikeController(
            QuestionsContext db,
            ILogger<QuestionController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /* Adds val to entity.Likes */
        [NonAction]
        public async Task<IActionResult> Change(Guid id, int val)
        {
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
