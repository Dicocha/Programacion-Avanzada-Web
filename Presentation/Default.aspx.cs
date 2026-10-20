using System;
using System.Globalization;
using System.Linq;
using System.Web.Services;
using Domain.Enums;
using Domain.ViewModels;
using Infrastructure.Repositories;
using RegistrarProducto = Application.UseCases.Registrar;
using ConsultarInventario = Application.UseCases.Consultar;
using CalcularInventario = Application.UseCases.CalcularValorInventario;

namespace Presentation
{
    public partial class Default : System.Web.UI.Page
    {
        [WebMethod]
        public static object ObtenerCategorias()
        {
            return new { ok = true, categorias = CategoriaProducto.Todas };
        }

        [WebMethod(EnableSession = true)]
        public static object Registrar(string nombre, string precio, string cantidad, string categoria)
        {
            try
            {
                decimal precioValor;
                if (!decimal.TryParse(precio, NumberStyles.Number, CultureInfo.InvariantCulture, out precioValor))
                    return new { ok = false, mensaje = "El precio tiene un formato inválido." };

                int cantidadValor;
                if (!int.TryParse(cantidad, NumberStyles.Integer, CultureInfo.InvariantCulture, out cantidadValor))
                    return new { ok = false, mensaje = "La cantidad debe ser un número entero." };

                var modelo = new ProductoViewModel
                {
                    Nombre = (nombre ?? string.Empty).Trim(),
                    Precio = precioValor,
                    Cantidad = cantidadValor,
                    Categoria = categoria
                };

                var casoUso = new RegistrarProducto(new ProductoRepository());
                casoUso.Ejecutar(modelo);

                return new { ok = true, mensaje = string.Format("Producto \"{0}\" registrado correctamente.", modelo.Nombre) };
            }
            catch (ArgumentException ex)
            {
                return new { ok = false, mensaje = ex.Message };
            }
            catch (Exception)
            {
                return new { ok = false, mensaje = "Ocurrió un error inesperado al registrar el producto." };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object Consultar(string estado, string categoria)
        {
            try
            {
                var casoUso = new ConsultarInventario(new ProductoRepository());
                var productos = casoUso.Ejecutar(estado, categoria);

                return new
                {
                    ok = true,
                    total = productos.Count,
                    productos = productos.Select(p => new
                    {
                        nombre = p.Nombre,
                        categoria = p.Categoria,
                        precio = p.Precio,
                        cantidad = p.Cantidad,
                        valorTotal = p.ValorTotal,
                        estado = p.Estado
                    }).ToList()
                };
            }
            catch (Exception)
            {
                return new { ok = false, mensaje = "Ocurrió un error al consultar el inventario." };
            }
        }

        [WebMethod(EnableSession = true)]
        public static object CalcularResumen()
        {
            try
            {
                var casoUso = new CalcularInventario(new ProductoRepository());
                var resumen = casoUso.Ejecutar();

                return new
                {
                    ok = true,
                    resumen = new
                    {
                        totalProductos = resumen.TotalProductos,
                        totalUnidades = resumen.TotalUnidades,
                        valorInventario = resumen.ValorTotalInventario,
                        agotados = resumen.ProductosAgotados,
                        porCategoria = resumen.PorCategoria.Select(c => new
                        {
                            categoria = c.Categoria,
                            cantidadProductos = c.CantidadProductos,
                            unidades = c.Unidades,
                            valorTotal = c.ValorTotal
                        }).ToList()
                    }
                };
            }
            catch (Exception)
            {
                return new { ok = false, mensaje = "Ocurrió un error al calcular el resumen del inventario." };
            }
        }
    }
}
