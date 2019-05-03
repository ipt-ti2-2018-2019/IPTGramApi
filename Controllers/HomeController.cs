using Microsoft.AspNetCore.Mvc;

namespace IPTGram.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}