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
    public class ModeratorController : Controller
    {
        private readonly QuestionsContext _db;
        private readonly ILogger<QuestionController> _logger;
        private readonly UserManager<User> _userManager;
        public ModeratorController(
            QuestionsContext db,
            ILogger<QuestionController> logger,
            UserManager<User> userManager) {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Moderator/
        public IActionResult Index() => View();
        // GET: Moderator/ShowHated
        public async Task<IActionResult> ShowHatedAsync()
        {
            var quesitons = await _db.Questions.Where(p => p.Likes < 0).OrderBy(p => p.Likes).ToListAsync();
            return View(quesitons);
        }
    }
}
