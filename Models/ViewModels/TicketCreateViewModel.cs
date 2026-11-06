using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Web.Models.ViewModels
{
    public class TicketCreateViewModel
    {
        [Required(ErrorMessage = "Ingresá un título.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Ingresá una descripción.")]
        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [StringLength(100, ErrorMessage = "La categoría no puede superar los 100 caracteres.")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }
    }
}
