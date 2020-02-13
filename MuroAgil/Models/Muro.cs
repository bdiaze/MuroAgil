using System;
using System.Collections.Generic;

namespace MuroAgil.Models
{
    public partial class Muro
    {
        public Muro()
        {
            Etapa = new HashSet<Etapa>();
            UsuarioMuro = new HashSet<UsuarioMuro>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }

        public ICollection<Etapa> Etapa { get; set; }
        public ICollection<UsuarioMuro> UsuarioMuro { get; set; }
    }
}
