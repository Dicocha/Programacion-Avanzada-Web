using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HelpDesk.Web.Models.ViewModels
{
    public class TicketDetailsViewModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Categoria { get; set; }

        public string Estado { get; set; }

        public int IdEstado { get; set; }

        public string Solicitante { get; set; }

        public string Asignado { get; set; }

        public int? IdUsuarioAsignado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public bool PuedeGestionar { get; set; }

        public IList<TicketCommentViewModel> Comentarios { get; set; }

        public IList<SelectListItem> EstadosDisponibles { get; set; }

        public IList<SelectListItem> UsuariosAsignables { get; set; }

        public CommentCreateViewModel NuevoComentario { get; set; }
    }
}
