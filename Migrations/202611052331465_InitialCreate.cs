namespace HelpDesk.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comentario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdTicket = c.Int(nullable: false),
                        IdUsuario = c.Int(nullable: false),
                        Texto = c.String(nullable: false, maxLength: 255),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ticket", t => t.IdTicket, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario)
                .Index(t => t.IdTicket)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Ticket",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 150),
                        Descripcion = c.String(nullable: false, maxLength: 255),
                        IdUsuarioSolicitante = c.Int(nullable: false),
                        IdUsuarioAsignado = c.Int(),
                        IdEstado = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaActualizacion = c.DateTime(nullable: false),
                        FechaResolucion = c.DateTime(),
                        Categoria = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EstadoTicket", t => t.IdEstado)
                .ForeignKey("dbo.Usuario", t => t.IdUsuarioAsignado)
                .ForeignKey("dbo.Usuario", t => t.IdUsuarioSolicitante)
                .Index(t => t.IdUsuarioSolicitante)
                .Index(t => t.IdUsuarioAsignado)
                .Index(t => t.IdEstado);
            
            CreateTable(
                "dbo.EstadoTicket",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nombre, unique: true, name: "IX_EstadoTicket_Nombre");
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 150),
                        Hash = c.Binary(nullable: false, maxLength: 255),
                        Rol = c.String(nullable: false, maxLength: 50),
                        FechaRegistro = c.DateTime(nullable: false),
                        Estado = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "IX_Usuario_Email");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comentario", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Comentario", "IdTicket", "dbo.Ticket");
            DropForeignKey("dbo.Ticket", "IdUsuarioSolicitante", "dbo.Usuario");
            DropForeignKey("dbo.Ticket", "IdUsuarioAsignado", "dbo.Usuario");
            DropForeignKey("dbo.Ticket", "IdEstado", "dbo.EstadoTicket");
            DropIndex("dbo.Usuario", "IX_Usuario_Email");
            DropIndex("dbo.EstadoTicket", "IX_EstadoTicket_Nombre");
            DropIndex("dbo.Ticket", new[] { "IdEstado" });
            DropIndex("dbo.Ticket", new[] { "IdUsuarioAsignado" });
            DropIndex("dbo.Ticket", new[] { "IdUsuarioSolicitante" });
            DropIndex("dbo.Comentario", new[] { "IdUsuario" });
            DropIndex("dbo.Comentario", new[] { "IdTicket" });
            DropTable("dbo.Usuario");
            DropTable("dbo.EstadoTicket");
            DropTable("dbo.Ticket");
            DropTable("dbo.Comentario");
        }
    }
}
