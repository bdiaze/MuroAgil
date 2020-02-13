using System;
using System.Collections.Generic;

namespace MuroAgil.Models
{
    public partial class UsuarioMuro
    {
        public int IdDuenno { get; set; }
        public int IdMuro { get; set; }
        public short Permiso { get; set; }

        public Usuario IdDuennoNavigation { get; set; }
        public Muro IdMuroNavigation { get; set; }

		public string NombrePermiso() {
			switch (Permiso) {
				case 1:
					return "Dueño";
				case 2:
					return "Edición";
				default:
					return "Lectura";
			}
		}

		public List<ValorNombre> NombresPermisos() {
			var lista = new List<ValorNombre> {
				new ValorNombre(1, "Dueño"),
				new ValorNombre(2, "Edición"),
				new ValorNombre(3, "Lectura")
			};
			return lista;
		}

		public class ValorNombre {
			public int valor;
			public string nombre;

			public ValorNombre(int valor, string nombre) {
				this.valor = valor;
				this.nombre = nombre;
			}
		}
    }
}
