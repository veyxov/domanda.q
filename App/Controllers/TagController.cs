using App.Models;
using App.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace App.Controllers
{
    public class TagController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;
        private readonly UserManager<User> _userManager;

        public TagController(QuestionsContext db, ILogger<QuestionController> logger, UserManager<User> userManager) {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Tag/Index
        public async Task<IActionResult> IndexAsync()
        {
            var tags = await _db.Tags.ToListAsync();
            return View(tags);
        }

        // GET: Tag/Create
        public IActionResult CreateAsync()
        {
            return View();
        }
        // POST: Tag/Create
        // BODY: App.Models.Tag
        [HttpPost]
        public async Task<IActionResult> CreateAsync(Tag tag)
        {
            if (!ModelState.IsValid)
                return View(tag);

            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Tag");
        }

        // GET: Tag/AddTagToQuestion
        public IActionResult AddTagToQuestion(string id) => View();

        // POST: Tag/AddTagToQuestion
        [HttpPost]
        public async Task<IActionResult> AddTagToQuestion(string id, Tag tag)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            var question = await _db.Questions.FindAsync(Guid.Parse(id));
            
            if (question == null) return NotFound("Question not found.");

            var existingTag = await _db.Tags.Where(p => p.Name == tag.Name).FirstOrDefaultAsync();

            var newTag = new Tag();
            newTag.Id = Guid.NewGuid();
            newTag.QuestionId = Guid.Parse(id);

            if (existingTag == null) {
                newTag.Name = tag.Name;
                newTag.Description = tag.Description;
            } else {
                newTag.Name = existingTag.Name;
                newTag.Description = existingTag.Description;
                _logger.Log(LogLevel.Warning, "Tag already exists!");
            }

            await _db.Tags.AddAsync(newTag);
            await _db.SaveChangesAsync();
            return RedirectToAction("Show", "Question", new { Id = id } );
        }
    }
}
