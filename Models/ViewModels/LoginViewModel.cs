using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingresá tu usuario o correo.")]
        [StringLength(150, ErrorMessage = "El usuario o correo no puede superar los 150 caracteres.")]
        [Display(Name = "Usuario o correo")]
        public string Identificador { get; set; }

        [Required(ErrorMessage = "Ingresá tu contraseña.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 100 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }

        public string ReturnUrl { get; set; }
    }
}
