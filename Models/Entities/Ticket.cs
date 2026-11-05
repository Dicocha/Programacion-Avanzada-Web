using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Web.Models.Entities
{
    [Table("Ticket")]
    public class Ticket
    {
        public Ticket()
        {
            Comentarios = new HashSet<Comentario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; }

        public int IdUsuarioSolicitante { get; set; }

        public int? IdUsuarioAsignado { get; set; }

        public int IdEstado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        [StringLength(100)]
        public string Categoria { get; set; }

        [ForeignKey("IdUsuarioSolicitante")]
        public virtual Usuario UsuarioSolicitante { get; set; }

        [ForeignKey("IdUsuarioAsignado")]
        public virtual Usuario UsuarioAsignado { get; set; }

        [ForeignKey("IdEstado")]
        public virtual EstadoTicket Estado { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; }
    }
}
