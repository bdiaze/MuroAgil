using System.ComponentModel.DataAnnotations;

namespace MuroAgil.ViewModels.Usuario {
    public class GenerarNuevaContrasenaViewModel {
        [Required(ErrorMessage = "Ocurrió un error al obtener la dirección de correo.")]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Ocurrió un error al obtener el token de recuperación")]
        public string TokenRecupContr { get; set; }

        [Display(Name = "Contraseña (8 caracteres mínimo)")]
        [Required(ErrorMessage = "No ha ingresado su nueva contraseña.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener 8 caracteres mínimo.")]
        public string Contrasenna { get; set; }

        [Display(Name = "Confirmar Contraseña")]
        [Required(ErrorMessage = "Se requiere la confirmación de la contraseña.")]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas ingresadas no coinciden.")]
        public string RepContrasenna { get; set; }
    }
}
