using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using HelpDesk.Web.Data;
using HelpDesk.Web.Models.Entities;
using HelpDesk.Web.Models.ViewModels;

namespace HelpDesk.Web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly HelpDeskContext _context = new HelpDeskContext();

        public ActionResult Index()
        {
            var usuarioId = ObtenerUsuarioId();
            var esAdministrador = User.IsInRole("Administrador");
            var query = _context.Tickets.AsNoTracking().AsQueryable();

            if (!esAdministrador)
            {
                query = query.Where(t => t.IdUsuarioSolicitante == usuarioId);
            }

            var tickets = query
                .OrderByDescending(t => t.FechaCreacion)
                .Select(t => new TicketListItemViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Categoria = t.Categoria,
                    Estado = t.Estado.Nombre,
                    Solicitante = t.UsuarioSolicitante.Nombre,
                    Asignado = t.UsuarioAsignado != null ? t.UsuarioAsignado.Nombre : "Sin asignar",
                    FechaCreacion = t.FechaCreacion,
                    FechaActualizacion = t.FechaActualizacion
                })
                .ToList();

            return View(tickets);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new TicketCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketCreateViewModel model)
        {
            if (model == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var estadoAbierto = _context.EstadosTicket.FirstOrDefault(e => e.Nombre == "Abierto");
            if (estadoAbierto == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontró el estado inicial del ticket.");
                return View(model);
            }

            var ahora = DateTime.Now;
            var ticket = new Ticket
            {
                Titulo = model.Titulo.Trim(),
                Descripcion = model.Descripcion.Trim(),
                Categoria = string.IsNullOrWhiteSpace(model.Categoria) ? null : model.Categoria.Trim(),
                IdUsuarioSolicitante = ObtenerUsuarioId(),
                IdEstado = estadoAbierto.Id,
                FechaCreacion = ahora,
                FechaActualizacion = ahora
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = ticket.Id });
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var usuarioId = ObtenerUsuarioId();
            var esAdministrador = User.IsInRole("Administrador");

            var ticket = _context.Tickets
                .AsNoTracking()
                .Include(t => t.UsuarioSolicitante)
                .Include(t => t.UsuarioAsignado)
                .Include(t => t.Estado)
                .Include(t => t.Comentarios.Select(c => c.Usuario))
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null || (!esAdministrador && ticket.IdUsuarioSolicitante != usuarioId))
            {
                return HttpNotFound();
            }

            var model = new TicketDetailsViewModel
            {
                Id = ticket.Id,
                Titulo = ticket.Titulo,
                Descripcion = ticket.Descripcion,
                Categoria = ticket.Categoria,
                Estado = ticket.Estado.Nombre,
                IdEstado = ticket.IdEstado,
                Solicitante = ticket.UsuarioSolicitante.Nombre,
                Asignado = ticket.UsuarioAsignado != null ? ticket.UsuarioAsignado.Nombre : "Sin asignar",
                IdUsuarioAsignado = ticket.IdUsuarioAsignado,
                FechaCreacion = ticket.FechaCreacion,
                FechaActualizacion = ticket.FechaActualizacion,
                FechaResolucion = ticket.FechaResolucion,
                PuedeGestionar = esAdministrador,
                Comentarios = ticket.Comentarios
                    .OrderBy(c => c.Fecha)
                    .Select(c => new TicketCommentViewModel
                    {
                        Id = c.Id,
                        Texto = c.Texto,
                        Fecha = c.Fecha,
                        UsuarioNombre = c.Usuario.Nombre,
                        UsuarioRol = c.Usuario.Rol
                    })
                    .ToList(),
                NuevoComentario = new CommentCreateViewModel()
            };

            if (esAdministrador)
            {
                model.EstadosDisponibles = _context.EstadosTicket
                    .AsNoTracking()
                    .OrderBy(e => e.Id)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Nombre,
                        Selected = e.Id == ticket.IdEstado
                    })
                    .ToList();

                model.UsuariosAsignables = _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.Estado == "Activo")
                    .OrderBy(u => u.Nombre)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Nombre,
                        Selected = u.Id == ticket.IdUsuarioAsignado
                    })
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int id, [Bind(Prefix = "NuevoComentario")] CommentCreateViewModel model)
        {
            var usuarioId = ObtenerUsuarioId();
            var esAdministrador = User.IsInRole("Administrador");
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

            if (ticket == null || (!esAdministrador && ticket.IdUsuarioSolicitante != usuarioId))
            {
                return HttpNotFound();
            }

            if (model == null || !ModelState.IsValid || string.IsNullOrWhiteSpace(model.Texto))
            {
                TempData["Error"] = "Escribí un comentario válido.";
                return RedirectToAction("Details", new { id });
            }

            _context.Comentarios.Add(new Comentario
            {
                IdTicket = id,
                IdUsuario = usuarioId,
                Texto = model.Texto.Trim(),
                Fecha = DateTime.Now
            });

            ticket.FechaActualizacion = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Assign(int id, int? idUsuarioAsignado)
        {
            var ticket = _context.Tickets
                .Include(t => t.Estado)
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (idUsuarioAsignado.HasValue)
            {
                var asignado = _context.Usuarios
                    .FirstOrDefault(u => u.Id == idUsuarioAsignado.Value && u.Estado == "Activo");

                if (asignado == null)
                {
                    TempData["Error"] = "El usuario seleccionado no está disponible.";
                    return RedirectToAction("Details", new { id });
                }
            }

            ticket.IdUsuarioAsignado = idUsuarioAsignado;
            ticket.FechaActualizacion = DateTime.Now;

            if (idUsuarioAsignado.HasValue && ticket.Estado.Nombre == "Abierto")
            {
                var enProceso = _context.EstadosTicket.FirstOrDefault(e => e.Nombre == "En Proceso");
                if (enProceso != null)
                {
                    ticket.IdEstado = enProceso.Id;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult ChangeStatus(int id, int idEstado)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
            var estado = _context.EstadosTicket.FirstOrDefault(e => e.Id == idEstado);

            if (ticket == null || estado == null)
            {
                return HttpNotFound();
            }

            ticket.IdEstado = estado.Id;
            ticket.FechaActualizacion = DateTime.Now;
            ticket.FechaResolucion = estado.Nombre == "Resuelto" || estado.Nombre == "Cerrado"
                ? DateTime.Now
                : (DateTime?)null;

            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

        private int ObtenerUsuarioId()
        {
            var identity = User.Identity as ClaimsIdentity;
            var valor = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int usuarioId;

            return int.TryParse(valor, out usuarioId) ? usuarioId : 0;
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
