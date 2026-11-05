using System;
using System.Linq;
using HelpDesk.Web.Models.Entities;
using HelpDesk.Web.Services;

namespace HelpDesk.Web.Data
{
    public static class SeedData
    {
        public static void Initialize(HelpDeskContext context)
        {
            var estados = new[] { "Abierto", "En Proceso", "Resuelto", "Cerrado" };

            foreach (var nombre in estados)
            {
                if (!context.EstadosTicket.Any(e => e.Nombre == nombre))
                {
                    context.EstadosTicket.Add(new EstadoTicket { Nombre = nombre });
                }
            }

            context.SaveChanges();

            CrearUsuario(context, "admin", "admin@helpdesk.local", "admin", "Administrador");
            CrearUsuario(context, "usuario", "usuario@helpdesk.local", "usuario", "Usuario");

            context.SaveChanges();
        }

        private static void CrearUsuario(
            HelpDeskContext context,
            string nombre,
            string email,
            string password,
            string rol)
        {
            if (context.Usuarios.Any(u => u.Email == email))
            {
                return;
            }

            context.Usuarios.Add(new Usuario
            {
                Nombre = nombre,
                Email = email,
                Hash = PasswordHasher.Hash(password),
                Rol = rol,
                FechaRegistro = DateTime.Now,
                Estado = "Activo"
            });
        }
    }
}
