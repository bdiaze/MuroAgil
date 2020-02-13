using System.ComponentModel.DataAnnotations;

namespace MuroAgil.ViewModels.Usuario {
    public class RecuperarContrasennaViewModel {
        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "Se requiere el ingreso del correo electrónico.")]
        [EmailAddress(ErrorMessage = "Correo electrónico con formato inválido.")]
        public string Correo { get; set; }
    }
}
