using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Model
{
    [Table("tb_tarjeta")]
    internal class tb_tarjeta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string descripcion { get; set; }
        public int idBanco { get; set; }
        public int idPropietario { get; set; }
        public string numero { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public int idTipoTarjeta { get; set; }
        public decimal? lineaCredito { get; set; }
    }
}
