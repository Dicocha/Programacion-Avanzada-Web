using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Web.Models.ViewModels
{
    public class CommentCreateViewModel
    {
        [Required(ErrorMessage = "Escribí un comentario.")]
        [StringLength(255, ErrorMessage = "El comentario no puede superar los 255 caracteres.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentario")]
        public string Texto { get; set; }
    }
}
