using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MuroAgil.Models
{
    public class RegistrarseViewModel : GoogleReCaptchaModelBase
    {
        [Display(Name = "Correo Electrónico")]
		[Required(ErrorMessage = "Se requiere el ingreso del correo electrónico.")]
        [EmailAddress(ErrorMessage = "Correo electrónico con formato inválido.")]
		public string Codigo { get; set; }

        [Display(Name = "Nombre")]
		[Required(ErrorMessage = "Se requiere el ingreso de su nombre.")]
		public string Nombre { get; set; }

        [Display(Name = "Contraseña")]
		[Required(ErrorMessage = "Se requiere la creación de una contraseña.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener 8 caracteres mínimo.")]
		public string Contrasenna { get; set; }

        [Display(Name = "Confirmar Contraseña")]
		[Required(ErrorMessage = "Se requiere la confirmación de la contraseña.")]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas ingresadas no coinciden.")]
		public string RepContrasenna { get; set; }
	}
}
