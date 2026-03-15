using Ezz_Store.PL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ezz_Store.PL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            return View();
        }

        public IActionResult Privacy()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
