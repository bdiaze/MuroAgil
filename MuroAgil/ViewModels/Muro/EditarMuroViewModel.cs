using MuroAgil.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuroAgil.Models {
    public class EditarMuroViewModel {

        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Se requiere el ingreso del nombre del muro.")]
        public string Nombre { get; set; }

        [Display(Name = "ETAPAS")]
        [EnsureOneElementValidation(ErrorMessage = "Se requiere el ingreso de por lo menos una etapa.")]
        public ICollection<Etapa> Etapas { get; set; }

        [Display(Name = "Etapa Eliminada")]
        [Required(ErrorMessage = "Se requiere definir lo que pasará con las tareas que se encontraban en las etapas eliminadas.")]
        public int AccionTareas { get; set; }

        public EditarMuroViewModel() {
            Etapas = new List<Etapa>();
        }

        public class Etapa {
            public int Id { get; set; }

            [Required(ErrorMessage = "Se requiere el ingreso del nombre de cada etapa.")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "Se requiere definir la posición de la etapa.")]
            public short Posicion { get; set; } 
        }
    }
}
