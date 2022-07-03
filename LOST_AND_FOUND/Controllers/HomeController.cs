using LOST_AND_FOUND.Data;
using LOST_AND_FOUND.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;

namespace LOST_AND_FOUND.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.FoundItem = _db.FoundItem.ToList();
            mymodel.LostItem = _db.LostItem.ToList();
            return _db.FoundItem != null ?
                          View(mymodel) :
                          Problem("Entity set 'ApplicationDbContext.User'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NoAcess()
        {
            return View();
        }

        public async Task<IActionResult> UserDetails(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return View(new UserViewModel
            {
                Email = user.Email,
                Contact = user.PhoneNumber,
                Id = user.Id,
                UserName= user.UserName,
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}