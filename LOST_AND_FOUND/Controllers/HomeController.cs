using LOST_AND_FOUND.Data;
using LOST_AND_FOUND.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LOST_AND_FOUND.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return _db.User != null ?
                          View(_db.User.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.User'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}