using System;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviePortal.Context;

namespace App.Controllers
{
    public class QuestionController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;
        private readonly UserManager<User> _userManager;

        public QuestionController(QuestionsContext db, ILogger<QuestionController> logger, UserManager<User> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet("Question/Show/{id}")]
        public async Task<IActionResult> ShowAsync(Guid id)
        {
            var question = _db.Questions.Where(p => p.Id == id).FirstOrDefault();

            return View(question);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(Question question)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            var curQuestion = new Question() {
                Id = Guid.NewGuid(),
                UserId = curUser.Id,
                Heading = question.Heading,
                Text = question.Text
            };

            await _db.Questions.AddAsync(curQuestion);
            await _db.SaveChangesAsync();

            return RedirectToAction("Show", "Question", new { Id = curQuestion.Id });
        }


        [Authorize]
        [HttpGet]
        public IActionResult Answer(Guid Id)
        {
            return View();
        }

        [Authorize]
        [HttpPost("Question/Answer/{id}")]
        //                                    Only Heading and Text are passed
        public async Task<IActionResult> AnswerAsync(string id, Answer answer)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            if (curUser == null) {
                throw new Exception("Current user not found");
            }

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
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Comment(string questionId)
        {
            return View();
        }

        [Authorize]
        [HttpPost("Question/Comment/{id}")]
        public async Task<IActionResult> CommentAsync(string id, Comment comment)
        {
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

        [Authorize]
        [HttpGet]
        public IActionResult CommentForAns(string questionId)
        {
            return View();
        }

        [Authorize]
        [HttpPost("Question/CommentForAns/{id}")]
        public async Task<IActionResult> CommentForAnsAsync(string id, Comment comment)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);

            var curComment = new Comment()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Text = comment.Text,
                User = curUser,
                UserId = curUser.Id,
                AnswerId = Guid.Parse(id),
            };

            await _db.Comments.AddAsync(curComment);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
