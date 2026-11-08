using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HelpDesk.Web.Models.ViewModels
{
    public class ReportViewModel
    {
        public string Estado { get; set; }

        public IList<SelectListItem> Estados { get; set; }

        public IList<ReportStatusSummaryViewModel> ResumenEstados { get; set; }

        public IList<ReportTicketItemViewModel> Tickets { get; set; }

        public int TotalTickets { get; set; }

        public int Pendientes { get; set; }

        public int ResueltosOCerrados { get; set; }
    }

    public class ReportStatusSummaryViewModel
    {
        public string Estado { get; set; }

        public int Cantidad { get; set; }

        public double Porcentaje { get; set; }
    }

    public class ReportTicketItemViewModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Solicitante { get; set; }

        public string Asignado { get; set; }

        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public double? HorasResolucion { get; set; }
    }
}
