using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels
{
    public class ProductoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 100 caracteres.")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser un número mayor a 0 con hasta 2 decimales.")]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, 1000000, ErrorMessage = "La cantidad debe estar entre 0 y 1,000,000.")]
        [Display(Name = "Cantidad disponible")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }
    }
}
