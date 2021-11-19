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
    public class ModeratorController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;
        private readonly UserManager<User> _userManager;
        public ModeratorController(QuestionsContext db, ILogger<QuestionController> logger, UserManager<User> userManager) {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ShowHatedAsync()
        {
            var quesitons = await _db.Questions.Where(p => p.Likes < 0).OrderBy(p => p.Likes).ToListAsync();
            _logger.Log(LogLevel.Critical, quesitons.Count().ToString());
            return View(quesitons);
        }
    }
}
