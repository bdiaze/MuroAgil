using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MuroAgil.Models
{
    public class IniciarSesionViewModel : GoogleReCaptchaModelBase
    {
        [Display(Name = "Correo Electrónico")]
		[Required(ErrorMessage = "Se requiere el ingreso del correo electrónico.")]
        [EmailAddress(ErrorMessage = "Correo electrónico con formato inválido.")]
        public string Codigo { get; set; }

        [Display(Name = "Contraseña")]
		[Required(ErrorMessage = "Se requiere el ingreso de la contraseña.")]
		public string Contrasenna { get; set; }
	}
}
