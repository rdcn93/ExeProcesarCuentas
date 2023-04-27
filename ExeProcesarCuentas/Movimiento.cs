using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas
{
    internal class Movimiento
    {
        public string Banco { get; set; }
        public string Descripcion { get; set; }
        public decimal Soles { get; set; }
        public decimal Dolares { get; set; }
        public bool Cuotas { get; set; }
        public bool Seguro { get; set; }
    }
}
