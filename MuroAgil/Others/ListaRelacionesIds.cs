using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuroAgil.Models {
    public class ListaRelacionesIds {
        public List<RelacionIds> ListaRelaciones { get; set; }

        public ListaRelacionesIds() {
            ListaRelaciones = new List<RelacionIds>();
        }

        public void Add(RelacionIds relacion) {
            ListaRelaciones.Add(relacion);
        }
    }
}
