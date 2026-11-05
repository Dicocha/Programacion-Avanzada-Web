using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Web.Models.Entities
{
    [Table("Usuario")]
    public class Usuario
    {
        public Usuario()
        {
            TicketsSolicitados = new HashSet<Ticket>();
            TicketsAsignados = new HashSet<Ticket>();
            Comentarios = new HashSet<Comentario>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150)]
        [Index("IX_Usuario_Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public byte[] Hash { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; }

        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; }

        public virtual ICollection<Ticket> TicketsSolicitados { get; set; }

        public virtual ICollection<Ticket> TicketsAsignados { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; }
    }
}
