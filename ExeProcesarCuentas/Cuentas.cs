using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace ExeProcesarCuentas
{
    internal class Cuentas
    {
        private int idTarjeta = 0;

        public bool ProcesarDescripcion(Movimiento mov)
        {
            if(!mov.Banco.IsNullOrEmpty() && !mov.Descripcion.IsNullOrEmpty())
            {
                if (mov.Cuotas || mov.Seguro)
                    return false;

                var lstDescripcion = mov.Descripcion.Split(' ').ToList();

                string strDescripcion = "";
                DateTime fechaMovimiento = DateTime.Today;
                DateTime fechaProcesamiento = DateTime.Today;

                int cantBorrarInicioDescripcion = 0;
                int cantBorrarFinalDescripcion = 0;

                string dateFormat = "";
                string dateDescription = "";

                try
                {
                    switch (mov.Banco.ToUpper())
                    {                        
                        case "INTERBANK":
                            var lstFecInterbank = lstDescripcion[0].Split('-');
                            dateDescription = lstFecInterbank[0] + "-" + MesADatetimeFormat(lstFecInterbank[1]);
                            dateFormat = "dd-MMM";
                            cantBorrarInicioDescripcion = 1;
                            cantBorrarFinalDescripcion = 2;
                            break;
                        case "DINERS":
                            dateDescription = lstDescripcion[0] + "-" + MesADatetimeFormat(lstDescripcion[1]);
                            dateFormat = "dd-MMM";
                            cantBorrarInicioDescripcion = 4;
                            cantBorrarFinalDescripcion = 2;
                            break;
                        case "BCP":                            
                            dateDescription = lstDescripcion[0] + ".";
                            dateFormat = "ddMMM";
                            cantBorrarInicioDescripcion = 2;
                            cantBorrarFinalDescripcion = 1;
                            break;
                        case "BBVA":
                            dateDescription = lstDescripcion[0];
                            dateFormat = "dd-MM";
                            cantBorrarInicioDescripcion = 2;
                            cantBorrarFinalDescripcion = 2;
                            break;
                    }
                                        
                    if (cantBorrarInicioDescripcion > 0)
                    {
                        for (int i = 1; i <= cantBorrarInicioDescripcion; i++)
                        {
                            lstDescripcion.RemoveAt(0);
                        }
                    }

                    if (cantBorrarFinalDescripcion > 0)
                    {
                        for (int i = 1; i <= cantBorrarFinalDescripcion; i++)
                        {
                            QuitarUltimoItemLista(ref lstDescripcion);
                        }
                    }

                    fechaMovimiento = DateTime.ParseExact(dateDescription, dateFormat, CultureInfo.CreateSpecificCulture("es"));
                    strDescripcion = String.Join(" ", lstDescripcion.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Linea: {mov.Linea}, Erorr {ex.Message}");
                    
                }                
            }            

            return true;
        }

        internal string MesADatetimeFormat(string mes)
        {
            if (!mes.IsNullOrEmpty())
            {
                string newMonth = UpFirstLetter(mes);
                return newMonth + ".";
            }

            return "";
        }

        internal string UpFirstLetter(string text)
        {
            if (!text.IsNullOrEmpty())
            {
                if (text.Length == 1)
                    return char.ToUpper(text[0]).ToString();
                else
                    return char.ToUpper(text[0]) + text.Substring(1).ToLower();
            }

            return text;
        }

        internal void QuitarUltimoItemLista(ref List<string> lstDescripcion)
        {
            if (lstDescripcion.Any()) //prevent IndexOutOfRangeException for empty list
            {
                lstDescripcion.RemoveAt(lstDescripcion.Count - 1);
            }
        }
    }
}
