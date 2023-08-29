using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas
{
    internal class Movimiento
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int Linea { get; set; }
        public string Banco { get; set; }
        public string Descripcion { get; set; }
        public decimal Soles { get; set; }
        public decimal Dolares { get; set; }
        public decimal Monto { get; set; }
        public bool Cuotas { get; set; }
        public bool Seguro { get; set; }
        public int idMoneda { get; set; }
        public MovimientoCuota Cuota { get; set; }
        public int idTarjeta { get; set; }
        public int idPeriodo { get; set; }
        public int idPersona { get; set; }
    }
}
