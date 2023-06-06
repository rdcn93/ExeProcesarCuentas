using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_cuenta")]
    internal class tb_cuenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string descripcion { get; set; }
        public int idTarjeta { get; set; }
        public int idMoneda { get; set; }
    }
}
