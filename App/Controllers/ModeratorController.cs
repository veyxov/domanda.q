using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class ModeratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
