using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_periodo_configuracion")]
    public class tb_periodo_configuracion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idTarjeta { get; set; }
        public int diaInicio { get; set; }
        public int diaCorte { get; set; }
        public int minDiaPago { get; set; }
        public int maxDiaPago { get; set; }
        public int estado { get; set; }
        public bool corteDiaHabil { get; set; }
    }
}
