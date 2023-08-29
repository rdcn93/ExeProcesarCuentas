using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_movimiento_cuota")]
    public class tb_movimiento_cuota
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idMovimiento { get; set; }
        public int nroCuota { get; set; }
        public int nroCuotas { get; set; }
        public int nroCuotasPorPagar { get; set; }
        public decimal TEA { get; set; }
        public decimal monto { get; set; }
        public decimal montoCuota { get; set; }
        public decimal montoPorPagar { get; set; }
        public decimal capital { get; set; }
        public decimal intereses { get; set; }
        public int idPeriodoCuota { get; set; }
    }
}
