using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;

namespace Application.UseCases
{
    public class Consultar
    {
        private readonly IProductoRepository _repositorio;

        public Consultar(IProductoRepository repositorio)
        {
            if (repositorio == null) throw new ArgumentNullException("repositorio");
            _repositorio = repositorio;
        }

        public IList<Producto> Ejecutar(string estado, string categoria)
        {
            IEnumerable<Producto> productos = _repositorio.GetAll();

            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                productos = productos.Where(p => p.Estado == estado);

            if (!string.IsNullOrWhiteSpace(categoria) && categoria != "Todas")
                productos = productos.Where(p => p.Categoria == categoria);

            return productos
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Nombre)
                .ToList();
        }
    }
}
