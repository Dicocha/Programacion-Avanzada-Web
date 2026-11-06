using System.Security.Claims;
using System.Web.Mvc;

namespace HelpDesk.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Usuario = User.Identity.Name;
            ViewBag.Rol = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }
    }
}
