using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Web.Models.Entities
{
    [Table("EstadoTicket")]
    public class EstadoTicket
    {
        public EstadoTicket()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index("IX_EstadoTicket_Nombre", IsUnique = true)]
        public string Nombre { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
