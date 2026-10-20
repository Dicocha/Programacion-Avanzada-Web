using System;
using System.Linq;
using Domain.Interfaces;
using Domain.ViewModels;

namespace Application.UseCases
{
    public class CalcularValorInventario
    {
        private readonly IProductoRepository _repositorio;

        public CalcularValorInventario(IProductoRepository repositorio)
        {
            if (repositorio == null) throw new ArgumentNullException("repositorio");
            _repositorio = repositorio;
        }

        public ResumenInventarioViewModel Ejecutar()
        {
            var productos = _repositorio.GetAll().ToList();

            return new ResumenInventarioViewModel
            {
                TotalProductos = productos.Count,
                TotalUnidades = productos.Sum(p => p.Cantidad),
                ValorTotalInventario = productos.Sum(p => p.ValorTotal),
                ProductosAgotados = productos.Count(p => p.Estado == "Agotado"),
                PorCategoria = productos
                    .GroupBy(p => p.Categoria)
                    .OrderBy(g => g.Key)
                    .Select(g => new CategoriaResumenViewModel
                    {
                        Categoria = g.Key,
                        CantidadProductos = g.Count(),
                        Unidades = g.Sum(p => p.Cantidad),
                        ValorTotal = g.Sum(p => p.ValorTotal)
                    })
                    .ToList()
            };
        }
    }
}
