using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, 1000000, ErrorMessage = "La cantidad debe estar entre 0 y 1,000,000.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Categoria { get; set; }

        public string Estado
        {
            get { return Cantidad > 0 ? "Disponible" : "Agotado"; }
        }

        public decimal ValorTotal
        {
            get { return Precio * Cantidad; }
        }
    }
}
