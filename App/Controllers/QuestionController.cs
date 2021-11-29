using System;
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
    public class QuestionController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;
        private readonly UserManager<User> _userManager;

        public QuestionController(QuestionsContext db, ILogger<QuestionController> logger, UserManager<User> userManager) {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Question/Show/Id
        [AllowAnonymous]
        public IActionResult Show(Guid id)
        {
            var question = _db.Questions.Where(p => p.Id == id).FirstOrDefault();

            if (question == null) return NotFound("Question not found !");

            return View(question);
        }

        // GET: Question/Create
        [Authorize]
        public IActionResult Create() => View();

        // POST: Question/Create
        // BODY: question
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(Question question)
        {
            if (!ModelState.IsValid) return View(question);

            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            var curQuestion = new Question() {
                Id = Guid.NewGuid(),
                UserId = curUser.Id,
                Heading = question.Heading,
                Text = question.Text,
                CreationDate = DateTime.UtcNow
            };

            await _db.Questions.AddAsync(curQuestion);
            await _db.SaveChangesAsync();

            return RedirectToAction("Show", "Question", new { Id = curQuestion.Id });
        }


        // GET: Question/Answer
        [Authorize]
        public IActionResult Answer(Guid Id) => View();

        // POST: question/Answer/id
        // ROUTE: id
        // BODY: answer
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AnswerAsync(string id, Answer answer)
        {
            if (!ModelState.IsValid) return View(answer);

            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            var curAnswer = new Answer()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,

                Heading = answer.Heading,
                Text = answer.Text,

                UserId = curUser.Id,
                QuestionId = Guid.Parse(id)
            };

            await _db.Answers.AddAsync(curAnswer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Show", "Question", new { Id = curAnswer.QuestionId } );
        }
        // GET: Question/Comment
        [Authorize]
        public IActionResult Comment(string questionId) => View();

        // POST: Question/Comment/id
        // ROUTE: id
        // BODY: comment
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CommentAsync(string id, Comment comment)
        {
            if (!ModelState.IsValid) return View(comment);
            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            var curComment = new Comment()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Text = comment.Text,
                User = curUser,
                UserId = curUser.Id,
                QuestionId = Guid.Parse(id)
            };

            await _db.Comments.AddAsync(curComment);
            _db.SaveChanges();
            return RedirectToAction("Show", "Question", new { Id = id });
        }

        // GET: Question/CommentForAns/id
        // ROUTE: questionId
        [Authorize]
        public IActionResult CommentForAns(string questionId) => View();

        // POST: Question/CommentForAns/Id
        // ROUTE: id
        // BODY: comment
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CommentForAnsAsync(string id, Comment comment)
        {
            if (!ModelState.IsValid) return View(comment);
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var answer = await _db.Answers.FindAsync(Guid.Parse(id));

            var curComment = new Comment()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Text = comment.Text,
                User = curUser,
                UserId = curUser.Id,
                AnswerId = answer.Id
            };

            await _db.Comments.AddAsync(curComment);
            _db.SaveChanges();
            return RedirectToAction("Show", "Question", new { Id = answer.QuestionId } );
        }

        // GET: Question/Edit/id
        // ROUTE: id
        [Authorize]
        public async Task<IActionResult> EditAsync(Guid id)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var question = await _db.Questions.FindAsync(id);
            if (question == null) return NotFound("Question not found !");

            if (!(await _userManager.IsInRoleAsync(curUser, "Admin")
                || await _userManager.IsInRoleAsync(curUser, "Moderator")) && 
                    question.UserId != curUser.Id)
                return Unauthorized("You are not the poster of this post !");

            return View(question);
        }

        // POST: Question/Edit/id
        // ROUTE: id
        // BODY: question
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPostAsync(Guid id, Question question)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            question.Text += $"<small><sub>Edited by {curUser.UserName} on {DateTime.UtcNow}</sub></small>";

            var newQuestion = await _db.Questions.FindAsync(id);
            newQuestion.Heading = question.Heading;
            newQuestion.Text = question.Text;

            _db.SaveChanges();
            return RedirectToAction("Show", "Question", new { Id = id } );
        }

        // GET: Question/EditAnswer/id
        // ROUTE: id
        [Authorize]
        public async Task<IActionResult> EditAnswerAsync(Guid id)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var answer = await _db.Answers.FindAsync(id);

            if (answer == null) return NotFound("Answer not found !");

            if (!(await _userManager.IsInRoleAsync(curUser, "Admin")
                || await _userManager.IsInRoleAsync(curUser, "Moderator")) && 
                    answer.UserId != curUser.Id)
                return Unauthorized("You are not the poster of this post !");

            return View(answer);
        }

        // POST: Question/EditAnswer/id
        // ROUTE: id
        [Authorize]
        [HttpPost("Question/EditAnswer/{id}")]
        public async Task<IActionResult> EditAnswerAsync(Guid id, Question question)
        {
            var newAnswer = await _db.Answers.FindAsync(id);
            if (newAnswer == null) return NotFound("Question not found !");

            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            question.Text += $"<small><sub>Edited by {curUser.UserName} on {DateTime.UtcNow}</sub></small>";

            newAnswer.Heading = question.Heading;
            newAnswer.Text = question.Text;
            _db.SaveChanges();
            return RedirectToAction("Show", "Question", new { Id = newAnswer.QuestionId } );
        }

        // GET: Question/Delete/id
        // ROUTE: id
        [Authorize]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var question = await _db.Questions.FindAsync(id);
            if (question == null) return NotFound("Question not found !");

            // Remove dep
            question.Comments.Clear();
            question.Answers.Clear();
            question.Tags.Clear();


            _db.Remove(question);
            await _db.SaveChangesAsync();
            return RedirectToAction("ShowAll", "Question");
        }

        // GET: Question/DeleteAnswer/id
        // ROUTE: id
        [Authorize]
        public async Task<IActionResult> DeleteAnswerAsync(Guid id)
        {
            var answer = await _db.Answers.FindAsync(id);
            if (answer == null) return NotFound("Answer not found !");
            var question = await _db.Questions.FindAsync(answer.QuestionId);
            if (question == null) return NotFound("Question not found !");

            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(answer.User, "Admin")
                || await _userManager.IsInRoleAsync(answer.User, "Moderator")
                || curUser.Id == answer.User.Id)) return Unauthorized();


            // Remove dep
            answer.Comments.Clear();

            // If this answer is a solution
            // We need to unmark the check
            if (answer.IsSolution)
            {
                question.IsSolved = false;
                answer.IsSolution = false;
                answer.User.Points--;
            }

            _db.Answers.Remove(answer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Show", "Question", new { Id = answer.QuestionId } );
        }

        // GET: Question/MarkSolution
        // ROUTE: id
        [Authorize]
        public async Task<IActionResult> MarkSolutionAsync(string id)
        {
            var answer = await _db.Answers.FindAsync(Guid.Parse(id));
            if (answer == null) return NotFound("Answer not found !");
            var question = await _db.Questions.FindAsync(answer.QuestionId);
            if (question == null) return NotFound("Question not found !");

            answer.IsSolution = true;
            question.IsSolved = true;

            // A a point to the user
            answer.User.Points++;

            await _db.SaveChangesAsync();
            return RedirectToAction("Show", "Question", new { Id = question.Id } );
        }

        // GET: Question/ShowAll
        // ROUTE: sortOrder
        [AllowAnonymous]
        public async Task<IActionResult> ShowAllAsync(string sortOrder)
        {
            var questions = await (sortOrder == "votes" ? _db.Questions.OrderByDescending(p => p.Likes).ToListAsync() 
                    : _db.Questions.OrderByDescending(p => p.Answers.Count()).ToListAsync());
            return View(questions);
        }

        // GET: Question/Report/id
        //
        [Authorize]
        public async Task<IActionResult> ReportAsync(Guid id) {
            var post = new Post();
            // Check for both Questions and Answers
            post = await _db.Questions.FindAsync(id);
            if (post == null) {
                post = await _db.Answers.FindAsync(id);
                if (post == null) return NotFound($"Post not found with id: {id}");
            }
            
            return View(post);
        }

        // POST: Question/Report/{id}
        // ROUTE: id
        // BODY: Comment
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReportAsync(Guid id, Post post)
        {
            var curPost = new Post();
            // Check for both Questions and Answers
            curPost = await _db.Questions.FindAsync(id);
            if (curPost == null) {
                curPost = await _db.Answers.FindAsync(id);
                if (curPost == null) return NotFound($"Post not found with id: {id}");
            }
            // Report
            curPost.IsReported = true;
            curPost.ReportText = post.ReportText;
            await _db.SaveChangesAsync();

            return RedirectToAction("Show", "Question", new { Id = id });
        }
        // GET: Question/SortOut
        // ROUTE: id
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> SortOutAsync(Guid id)
        {
            var post = await _db.Questions.FindAsync(id);
            // Clear the report
            post.IsReported = false;
            post.ReportText = string.Empty;

            await _db.SaveChangesAsync();
            return RedirectToAction("Show", "Question", new { Id = id });
        }
    }
}
