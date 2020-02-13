using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuroAgil.Others {
    public class ExcepcionMuroAgil : Exception{
        public ExcepcionMuroAgil() {

        }

        public ExcepcionMuroAgil(string message) : base(message) {

        }

        public ExcepcionMuroAgil(string message, Exception inner) : base(message, inner) {

        }
    }
}
