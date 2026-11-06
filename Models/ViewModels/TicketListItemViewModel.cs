using System;

namespace HelpDesk.Web.Models.ViewModels
{
    public class TicketListItemViewModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Categoria { get; set; }

        public string Estado { get; set; }

        public string Solicitante { get; set; }

        public string Asignado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
