using cAPParel.ConsoleMVCClient.Models;
using cAPParel.ConsoleMVCClient.Views.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace cAPParel.ConsoleMVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var indexView = new Indeks();
            indexView.Render();
            Console.ReadKey();
            return RedirectToAction("Index");
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