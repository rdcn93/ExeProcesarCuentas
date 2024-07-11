using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Classes
{
    internal class MovimientoCuota
    {
        public int nroCuotas { get; set; }
        public int nroCuota { get; set; }
        public int nroCuotasPorPagar { get; set; }
        public decimal TEA { get; set; }
        public decimal monto { get; set; }
        public decimal montoCuota { get; set; }
        public decimal montoPorPagar { get; set; }
        public decimal capital { get; set; }
        public decimal intereses { get; set; }
    }
}
