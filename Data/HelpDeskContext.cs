using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using HelpDesk.Web.Models.Entities;

namespace HelpDesk.Web.Data
{
    public class HelpDeskContext : DbContext
    {
        public HelpDeskContext()
            : base("name=HelpDeskContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<EstadoTicket> EstadosTicket { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Usuario>()
                .ToTable("Usuario");

            modelBuilder.Entity<EstadoTicket>()
                .ToTable("EstadoTicket");

            modelBuilder.Entity<Ticket>()
                .ToTable("Ticket");

            modelBuilder.Entity<Comentario>()
                .ToTable("Comentario");

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.UsuarioSolicitante)
                .WithMany(u => u.TicketsSolicitados)
                .HasForeignKey(t => t.IdUsuarioSolicitante)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
                .HasOptional(t => t.UsuarioAsignado)
                .WithMany(u => u.TicketsAsignados)
                .HasForeignKey(t => t.IdUsuarioAsignado)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Estado)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.IdEstado)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comentario>()
                .HasRequired(c => c.Ticket)
                .WithMany(t => t.Comentarios)
                .HasForeignKey(c => c.IdTicket)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Comentario>()
                .HasRequired(c => c.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.IdUsuario)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
