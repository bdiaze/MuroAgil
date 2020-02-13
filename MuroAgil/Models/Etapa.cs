using System;
using System.Collections.Generic;

namespace MuroAgil.Models
{
    public partial class Etapa
    {
        public Etapa()
        {
            Tarea = new HashSet<Tarea>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdMuro { get; set; }
        public short Posicion { get; set; }

        public Muro IdMuroNavigation { get; set; }
        public ICollection<Tarea> Tarea { get; set; }
    }
}
