
using System.ComponentModel.DataAnnotations;

namespace MuroAgil.ViewModels {
    public class CorreoSinValidarViewModel {

        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "Se requiere el ingreso del correo electrónico.")]
        [EmailAddress(ErrorMessage = "Correo electrónico con formato inválido.")]
        public string Correo { get; set; }
    }
}
