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

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var questions = await _db.Questions.ToListAsync();
            return View(questions);
        }

        [HttpGet("Question/Show/{id}")]
        public async Task<IActionResult> Show(Guid id)
        {
            var question = _db.Questions.Where(p => p.Id == id).FirstOrDefault();

            QuestionDTO dto = new QuestionDTO() 
            {
                User = await _db.Users.FindAsync(question.UserId),
                Id = question.Id,
                CreationDate = question.CreationDate,
                Heading = question.Heading,
                Likes = question.Likes,
                Text = question.Text,
                Answers = await _db.Answers.Where(p => p.QuestionId == question.Id).ToListAsync()
            };

            if (dto.User == null) {
                throw new  Exception("Not foudn user !");
            }

            return View(dto);
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

            await _db.Questions.AddAsync(
                    new Question() {
                    Id = Guid.NewGuid(),
                    UserId = curUser.Id,
                    Heading = question.Heading, Text = question.Text, });
            await _db.SaveChangesAsync();

            return RedirectToAction("ShowAll", "Question");
        }


        [Authorize]
        [HttpGet]
        public IActionResult Answer(Guid Id)
        {
            return View();
        }

        [Authorize]
        [HttpPost("Question/Answer/{id}")]
        public async Task<IActionResult> AnswerAsync(string id, Answer answer)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var curAnswer = answer;
            curAnswer.QuestionId = Guid.Parse(id);
            curAnswer.Id = Guid.NewGuid();
            curAnswer.UserId = curUser.Id;
            await _db.Answers.AddAsync(curAnswer);
            await _db.SaveChangesAsync();
            return RedirectToAction("ShowAll", "Question");
        }
    }
}
