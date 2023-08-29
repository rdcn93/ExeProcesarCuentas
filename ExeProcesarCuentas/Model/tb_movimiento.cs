using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_movimiento")]
    public class tb_movimiento
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public decimal monto { get; set; }
        public bool enCuotas { get; set; }
        public int nroCuotas { get; set; }
        public int idTarjeta { get; set; }
        public int idMoneda { get; set; }
        public int? idPais { get; set; }
        public int? idCategoria { get; set; }
        public int? idPersonaMovimiento { get; set; }
        public int? idPeriodo { get; set; }
        public int? idCuenta { get; set; }
        public bool esDevagramen { get; set; }
    }
}
