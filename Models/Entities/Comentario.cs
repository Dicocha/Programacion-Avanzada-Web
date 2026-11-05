using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Web.Models.Entities
{
    [Table("Comentario")]
    public class Comentario
    {
        public int Id { get; set; }

        public int IdTicket { get; set; }

        public int IdUsuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Texto { get; set; }

        public DateTime Fecha { get; set; }

        [ForeignKey("IdTicket")]
        public virtual Ticket Ticket { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}
