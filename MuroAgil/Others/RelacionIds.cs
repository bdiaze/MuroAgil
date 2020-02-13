using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuroAgil.Models {
    public class RelacionIds {
        public int PseudoId { get; set; }
        public Tarea TareaReal { get; set; }

        public RelacionIds(int PseudoId, Tarea TareaReal) {
            this.PseudoId = PseudoId;
            this.TareaReal = TareaReal;
        }
    }
}
