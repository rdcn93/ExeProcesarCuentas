using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_tarjeta_periodo")]
    internal class tb_tarjeta_periodo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idTarjeta { get; set; }
        public int mes { get; set; }
        public int anio { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaCorte { get; set; }
        public DateTime fechaMinPago { get; set; }
        public DateTime fechaMaxPago { get; set; }
    }
}
