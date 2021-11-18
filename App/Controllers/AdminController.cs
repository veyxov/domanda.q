using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoviePortal.Context;

namespace App.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuestionsContext _db;

        public AdminController(ILogger<HomeController> logger, QuestionsContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
