using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class CategoriaResumenViewModel
    {
        public string Categoria { get; set; }
        public int CantidadProductos { get; set; }
        public int Unidades { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class ResumenInventarioViewModel
    {
        public ResumenInventarioViewModel()
        {
            PorCategoria = new List<CategoriaResumenViewModel>();
        }

        public int TotalProductos { get; set; }
        public int TotalUnidades { get; set; }
        public decimal ValorTotalInventario { get; set; }
        public int ProductosAgotados { get; set; }
        public IList<CategoriaResumenViewModel> PorCategoria { get; set; }
    }
}
