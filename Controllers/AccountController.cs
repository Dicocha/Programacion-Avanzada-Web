using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using HelpDesk.Web.Data;
using HelpDesk.Web.Models.ViewModels;
using HelpDesk.Web.Services;
using Microsoft.Owin.Security;

namespace HelpDesk.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly HelpDeskContext _context = new HelpDeskContext();

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Tickets");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var identificador = (model.Identificador ?? string.Empty).Trim();
            var usuario = _context.Usuarios.FirstOrDefault(
                u => u.Email == identificador || u.Nombre == identificador);

            if (usuario == null ||
                usuario.Estado != "Activo" ||
                !PasswordHasher.Verify(model.Password, usuario.Hash))
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(model);
            }

            var identidad = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            }, "HelpDeskCookie");

            var properties = new AuthenticationProperties
            {
                IsPersistent = model.Recordarme
            };

            if (model.Recordarme)
            {
                properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }

            HttpContext.GetOwinContext().Authentication.SignIn(properties, identidad);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Tickets");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("HelpDeskCookie");
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
