using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_prestamo")]
    public class tb_prestamo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public int idCuenta { get; set; }
        public int idBeneficiario { get; set; }
        public int estado { get; set; }
    }
}
