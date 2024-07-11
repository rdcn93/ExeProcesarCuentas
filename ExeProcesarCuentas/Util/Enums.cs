using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Util
{
    internal class Enums
    {
        public enum MovimientoTipo : int
        {
            Credito = 4,
            Debito = 5,
            Efectivo = 6
        }

        public enum Moneda : int
        {
            Soles = 1,
            Dolares = 2
        }

        public enum MovimientoAccion : int
        {
            Registrar = 1,
            Actualizar = 2,
            Eliminar = 3
        }

        public enum CreditoPos : int
        {
            Mes = 1,
            Banco = 2,
            Descripcion = 3,
            Soles = 4,
            Dolares = 5,
            EsCuotas = 6,
            Pertenece = 7,
            Categoria = 8,
            Fecha = 9,
            Procesado = 10,
            NroCuotas = 11,
            NroCuota = 12,
            DescripcionOriginal = 13
        }

        public enum DebitoSheetPosition : int
        {
            Registrar = 1,
            Actualizar = 2,
            Eliminar = 3
        }
    }
}
