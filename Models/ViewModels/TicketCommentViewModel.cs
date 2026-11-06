using System;

namespace HelpDesk.Web.Models.ViewModels
{
    public class TicketCommentViewModel
    {
        public int Id { get; set; }

        public string Texto { get; set; }

        public DateTime Fecha { get; set; }

        public string UsuarioNombre { get; set; }

        public string UsuarioRol { get; set; }
    }
}
