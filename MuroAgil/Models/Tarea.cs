using System;
using System.Collections.Generic;

namespace MuroAgil.Models
{
    public partial class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int IdEtapa { get; set; }
        public short Posicion { get; set; }
        public short Familia { get; set; }

        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }

        public Etapa Etapa { get; set; }
    }
}
