using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Classes
{
    internal class Movimiento
    {
        public int Id { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int Linea { get; set; }
        public string Banco { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionOriginal { get; set; }
        public decimal Soles { get; set; }
        public decimal Dolares { get; set; }
        public decimal Monto { get; set; }
        public bool Cuotas { get; set; }
        public bool Seguro { get; set; }
        public int idMoneda { get; set; }
        public MovimientoCuota Cuota { get; set; }
        public int? idTarjeta { get; set; }
        public int? idCuenta { get; set; }
        public int idPeriodo { get; set; }
        public int idPersona { get; set; }
        public int? idCategoria { get; set; }
        public bool DescripcionConInfo { get; set; }
        public int idTipoMovimiento { get; set; }
        public bool esPrestamo { get; set; }
        public int idPersonaPrestamo { get; set; }
        public DateTime? fechaPrestamo { get; set; }
        public bool esDeposito { get; set; }
        public int idBancoDeposito { get; set; }
        public bool esPagoCredito { get; set; }
        public int idTarjetaCredito { get; set; }
        public int idPeriodoPago { get; set; }
        public int accion { get; set; }
        public int Row { get; set; }
        public int Cell { get; set; }
        public bool Procesado { get; set; }
        public bool ResultProcesarDescripcion { get; set; }
        public string SheetName { get; set; }
    }
}
