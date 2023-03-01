using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using WebAppMvc.Models;

namespace WebAppMvc.Controllers {

    [Route("[controller]/[action]")]
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [Route("/")]
        [Route("/[controller]/[action]")]
        public IActionResult Index() {
            throw new NullReferenceException();
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}