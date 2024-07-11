using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_tarjeta_periodo_pago")]
    public class tb_tarjeta_periodo_pago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idPeriodo { get; set; }
        public int idMovimiento { get; set; }
        public decimal monto { get; set; }
    }
}
