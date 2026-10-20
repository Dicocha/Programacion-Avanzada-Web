using System;
using System.Collections.Generic;
using System.Web;
using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private const string ClaveSesion = "Inventario";

        public void Add(Producto producto)
        {
            if (producto == null) throw new ArgumentNullException("producto");

            var productos = ObtenerLista();
            producto.Id = productos.Count == 0 ? 1 : productos[productos.Count - 1].Id + 1;
            productos.Add(producto);
            GuardarLista(productos);
        }

        public IList<Producto> GetAll()
        {
            return ObtenerLista();
        }

        private static IList<Producto> ObtenerLista()
        {
            var contexto = HttpContext.Current;
            if (contexto == null || contexto.Session == null)
                return new List<Producto>();

            var productos = contexto.Session[ClaveSesion] as IList<Producto>;
            if (productos == null)
            {
                productos = new List<Producto>();
                contexto.Session[ClaveSesion] = productos;
            }

            return productos;
        }

        private static void GuardarLista(IList<Producto> productos)
        {
            var contexto = HttpContext.Current;
            if (contexto != null && contexto.Session != null)
                contexto.Session[ClaveSesion] = productos;
        }
    }
}
