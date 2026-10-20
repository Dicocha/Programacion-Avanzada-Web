using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
