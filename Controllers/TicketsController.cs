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

        public ActionResult Index(string buscar, string estado)
        {
            buscar = string.IsNullOrWhiteSpace(buscar) ? null : buscar.Trim();
            estado = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim();

            var usuarioId = ObtenerUsuarioId();
            var esAdministrador = User.IsInRole("Administrador");
            var query = _context.Tickets.AsNoTracking().AsQueryable();

            if (!esAdministrador)
            {
                query = query.Where(t => t.IdUsuarioSolicitante == usuarioId);
            }

            if (buscar != null)
            {
                var termino = buscar.TrimStart('#').Trim();
                int numero;

                if (int.TryParse(termino, out numero))
                {
                    query = query.Where(t => t.Id == numero || t.Titulo.Contains(buscar));
                }
                else
                {
                    query = query.Where(t => t.Titulo.Contains(buscar));
                }
            }

            if (estado != null)
            {
                query = query.Where(t => t.Estado.Nombre == estado);
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

            if (Request.IsAjaxRequest())
            {
                return PartialView("_TicketTable", tickets);
            }

            var model = new TicketsIndexViewModel
            {
                Tickets = tickets,
                Buscar = buscar,
                Estado = estado,
                Estados = _context.EstadosTicket
                    .AsNoTracking()
                    .OrderBy(e => e.Id)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Nombre,
                        Text = e.Nombre,
                        Selected = e.Nombre == estado
                    })
                    .ToList()
            };

            ViewData["estado"] = estado;

            return View(model);
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
                if (Request.IsAjaxRequest())
                {
                    return Json(new { ok = false, message = "No se encontró el ticket solicitado." });
                }

                return HttpNotFound();
            }

            if (model == null || !ModelState.IsValid || string.IsNullOrWhiteSpace(model.Texto))
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { ok = false, message = "Escribí un comentario válido." });
                }

                TempData["Error"] = "Escribí un comentario válido.";
                return RedirectToAction("Details", new { id });
            }

            var texto = model.Texto.Trim();
            var ahora = DateTime.Now;

            _context.Comentarios.Add(new Comentario
            {
                IdTicket = id,
                IdUsuario = usuarioId,
                Texto = texto,
                Fecha = ahora
            });

            ticket.FechaActualizacion = ahora;
            _context.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    ok = true,
                    comentario = new
                    {
                        usuarioNombre = User.Identity.Name,
                        usuarioRol = ObtenerUsuarioRol(),
                        texto = texto,
                        fecha = ahora.ToString("dd/MM/yyyy HH:mm")
                    },
                    fechaActualizacion = ahora.ToString("dd/MM/yyyy HH:mm")
                });
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Assign(int id, int? idUsuarioAsignado)
        {
            var ticket = _context.Tickets
                .Include(t => t.Estado)
                .Include(t => t.UsuarioAsignado)
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { ok = false, message = "No se encontró el ticket solicitado." });
                }

                return HttpNotFound();
            }

            Usuario asignado = null;

            if (idUsuarioAsignado.HasValue)
            {
                asignado = _context.Usuarios
                    .FirstOrDefault(u => u.Id == idUsuarioAsignado.Value && u.Estado == "Activo");

                if (asignado == null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { ok = false, message = "El usuario seleccionado no está disponible." });
                    }

                    TempData["Error"] = "El usuario seleccionado no está disponible.";
                    return RedirectToAction("Details", new { id });
                }
            }

            ticket.IdUsuarioAsignado = idUsuarioAsignado;
            ticket.UsuarioAsignado = asignado;
            ticket.FechaActualizacion = DateTime.Now;

            if (idUsuarioAsignado.HasValue && ticket.Estado.Nombre == "Abierto")
            {
                var enProceso = _context.EstadosTicket.FirstOrDefault(e => e.Nombre == "En Proceso");
                if (enProceso != null)
                {
                    ticket.IdEstado = enProceso.Id;
                    ticket.Estado = enProceso;
                }
            }

            _context.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                return Json(CrearRespuestaGestion(ticket, "Asignación guardada."));
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult ChangeStatus(int id, int idEstado)
        {
            var ticket = _context.Tickets
                .Include(t => t.Estado)
                .Include(t => t.UsuarioAsignado)
                .FirstOrDefault(t => t.Id == id);

            var estado = _context.EstadosTicket.FirstOrDefault(e => e.Id == idEstado);

            if (ticket == null || estado == null)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { ok = false, message = "No se pudo actualizar el estado del ticket." });
                }

                return HttpNotFound();
            }

            var ahora = DateTime.Now;
            var estadoResuelto = estado.Nombre == "Resuelto" || estado.Nombre == "Cerrado";

            ticket.IdEstado = estado.Id;
            ticket.Estado = estado;
            ticket.FechaActualizacion = ahora;

            if (estadoResuelto)
            {
                if (!ticket.FechaResolucion.HasValue)
                {
                    ticket.FechaResolucion = ahora;
                }
            }
            else
            {
                ticket.FechaResolucion = null;
            }

            _context.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                return Json(CrearRespuestaGestion(ticket, "Estado actualizado."));
            }

            return RedirectToAction("Details", new { id });
        }

        private object CrearRespuestaGestion(Ticket ticket, string mensaje)
        {
            return new
            {
                ok = true,
                message = mensaje,
                idEstado = ticket.IdEstado,
                estado = ticket.Estado != null ? ticket.Estado.Nombre : string.Empty,
                idUsuarioAsignado = ticket.IdUsuarioAsignado,
                asignado = ticket.UsuarioAsignado != null ? ticket.UsuarioAsignado.Nombre : "Sin asignar",
                fechaActualizacion = ticket.FechaActualizacion.ToString("dd/MM/yyyy HH:mm"),
                fechaResolucion = ticket.FechaResolucion.HasValue
                    ? ticket.FechaResolucion.Value.ToString("dd/MM/yyyy HH:mm")
                    : null
            };
        }

        private string ObtenerUsuarioRol()
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
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
