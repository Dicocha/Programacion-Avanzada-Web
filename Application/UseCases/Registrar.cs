using System;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Domain.ViewModels;

namespace Application.UseCases
{
    public class Registrar
    {
        private readonly IProductoRepository _repositorio;

        public Registrar(IProductoRepository repositorio)
        {
            if (repositorio == null) throw new ArgumentNullException("repositorio");
            _repositorio = repositorio;
        }

        public void Ejecutar(ProductoViewModel modelo)
        {
            if (modelo == null) throw new ArgumentNullException("modelo");
            Validar(modelo);

            var producto = new Producto
            {
                Nombre = modelo.Nombre.Trim(),
                Precio = modelo.Precio,
                Cantidad = modelo.Cantidad,
                Categoria = modelo.Categoria
            };

            _repositorio.Add(producto);
        }

        private static void Validar(ProductoViewModel modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.Nombre) || modelo.Nombre.Trim().Length > 100)
                throw new ArgumentException("El nombre del producto es obligatorio y no puede superar los 100 caracteres.");

            if (modelo.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0.");

            if (modelo.Cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa.");

            if (!CategoriaProducto.EsValida(modelo.Categoria))
                throw new ArgumentException("La categoría seleccionada no es válida.");
        }
    }
}
