using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using HelpDesk.Web.Data;
using HelpDesk.Web.Models.ViewModels;

namespace HelpDesk.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportsController : Controller
    {
        private readonly HelpDeskContext _context = new HelpDeskContext();

        public ActionResult Index(string estado)
        {
            estado = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim();

            var tickets = _context.Tickets
                .AsNoTracking()
                .OrderByDescending(t => t.FechaCreacion)
                .Select(t => new
                {
                    t.Id,
                    t.Titulo,
                    Solicitante = t.UsuarioSolicitante.Nombre,
                    Asignado = t.UsuarioAsignado != null ? t.UsuarioAsignado.Nombre : "Sin asignar",
                    Estado = t.Estado.Nombre,
                    t.FechaCreacion,
                    t.FechaActualizacion,
                    t.FechaResolucion
                })
                .ToList()
                .Select(t => new ReportTicketItemViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Solicitante = t.Solicitante,
                    Asignado = t.Asignado,
                    Estado = t.Estado,
                    FechaCreacion = t.FechaCreacion,
                    FechaActualizacion = t.FechaActualizacion,
                    FechaResolucion = t.FechaResolucion,
                    HorasResolucion = t.FechaResolucion.HasValue
                        ? (t.FechaResolucion.Value - t.FechaCreacion).TotalHours
                        : (double?)null
                })
                .ToList();

            var ticketsFiltrados = estado == null
                ? tickets
                : tickets.Where(t => t.Estado == estado).ToList();

            var estados = _context.EstadosTicket
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Select(e => e.Nombre)
                .ToList();

            var totalTickets = tickets.Count;

            var resumenEstados = estados
                .Select(nombre =>
                {
                    var cantidad = tickets.Count(t => t.Estado == nombre);

                    return new ReportStatusSummaryViewModel
                    {
                        Estado = nombre,
                        Cantidad = cantidad,
                        Porcentaje = totalTickets == 0
                            ? 0
                            : cantidad * 100.0 / totalTickets
                    };
                })
                .ToList();

            var model = new ReportViewModel
            {
                Estado = estado,
                Estados = estados
                    .Select(nombre => new SelectListItem
                    {
                        Value = nombre,
                        Text = nombre,
                        Selected = nombre == estado
                    })
                    .ToList(),
                ResumenEstados = resumenEstados,
                Tickets = ticketsFiltrados,
                TotalTickets = totalTickets,
                Pendientes = tickets.Count(t => !t.FechaResolucion.HasValue),
                ResueltosOCerrados = tickets.Count(t => t.FechaResolucion.HasValue)
            };

            return View(model);
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
