using ClosedXML.Excel;
using ExeProcesarCuentas.Classes;
using ExeProcesarCuentas.Data;
using ExeProcesarCuentas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using static ExeProcesarCuentas.Util.Enums;

namespace ExeProcesarCuentas.Logic
{
    internal class Excel
    {
        private Cuentas cuentasClass = new Cuentas();
        IConfigurationRoot configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public bool ResetearValoresExcelCompleto()
        {
            bool result = false;
            var excelFilePath = configBuilder.GetSection("Paths:ExcelFilePath").Value;

            using (XLWorkbook workBook = new XLWorkbook(excelFilePath))
            {
                foreach (IXLWorksheet workSheet in workBook.Worksheets)
                {
                    if (workSheet.Position != 1 && workSheet.Position != 2)
                        continue;

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    int linea = 1;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                        }
                        else
                        {
                            try
                            {
                                string descripcionOriginal = row.Cell(14).Value.ToString();
                                bool procesado = !row.Cell(11).Value.ToString().IsNullOrEmpty();
                                bool cuotasBool = row.Cell(7).Value.ToString().IsNullOrEmpty() ? false : true;

                                if (cuotasBool && procesado && !descripcionOriginal.IsNullOrEmpty())
                                {
                                    workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value = descripcionOriginal;
                                    workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, 11).Value = "";
                                    workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, 14).Value = "";
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Linea: " + linea + " - " + ex.ToString());
                            }

                            linea++;
                        }
                    }

                    workBook.Save();
                }
            }

            return result;
        }

        public List<Movimiento> ObtenerMovimientosExcelCompleto()
        {
            var excelFilePath = configBuilder.GetSection("Paths:ExcelFilePath").Value;
            List<Movimiento> movimientos = new List<Movimiento>();

            #region db
            List<tb_tarjeta> lstTarjetas = new List<tb_tarjeta>();
            List<tb_moneda> lstMonedas = new List<tb_moneda>();
            List<tb_banco> lstBancos = new List<tb_banco>();
            List<tb_pais> lstPaises = new List<tb_pais>();
            List<tb_categoria> lstCategoria = new List<tb_categoria>();
            List<tb_persona> lstPersonas = new List<tb_persona>();
            List<tb_tarjeta_periodo> lstPeriodos = new List<tb_tarjeta_periodo>();
            List<tb_cuenta> lstCuentas = new List<tb_cuenta>();

            List<tb_movimiento> currentMovimientos = new List<tb_movimiento>();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                    .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                    .Options;
            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                lstMonedas = cuentasDbContext.monedas?.ToList();
                lstPaises = cuentasDbContext.paises?.ToList();
                lstTarjetas = cuentasDbContext.tarjetas?.ToList();
                lstPersonas = cuentasDbContext.personas?.ToList();
                lstCategoria = cuentasDbContext.categorias?.ToList();
                lstPeriodos = cuentasDbContext.periodos?.ToList();
                lstBancos = cuentasDbContext.bancos.ToList();
                lstCuentas = cuentasDbContext.cuentas.ToList();
                currentMovimientos = cuentasDbContext.movimientos.ToList();
            } 
            #endregion

            int idPropietario = lstPersonas?.Where(x => x.apePaterno.Trim().Equals("Castañeda") &&
                                        x.apeMaterno.Trim().Equals("Najar") &&
                                        x.nombre1.Trim().Equals("Raul")
                                        )?.FirstOrDefault()?.id ?? 0;

            int idCategoriaSeguro = lstCategoria.Where(x => x.descripcion.Equals("Seguro de Desgravamen")).FirstOrDefault()?.id ?? 1030;

            using (XLWorkbook workBook = new XLWorkbook(excelFilePath))
            {
                foreach (IXLWorksheet workSheet in workBook.Worksheets)
                {
                    bool esCredito = false;
                    int año = 0;
                    if (int.TryParse(workSheet.Name, out año))
                        esCredito = true;

                    //if (año == 0 && workSheet.Name != "Debito")
                    //    continue;

                    if (año == 0)
                        continue;

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    int linea = 1;

                    foreach (IXLRow row in workSheet.Rows())
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                        }
                        else
                        {
                            try
                            {
                                #region Variables
                                int mes = 0;
                                string banco = "";
                                string descripcion = "";
                                decimal monto = 0;
                                decimal solesDecimal = 0;
                                decimal dolaresDecimal = 0;
                                bool cuotasBool = false;
                                bool seguroBool = false;
                                string pertence = "";
                                string categoria = "";
                                bool descripcionConInfo = false;
                                DateTime fecha = DateTime.Today;
                                int idMoneda = 0;
                                int idTipoMovimiento = (int)MovimientoTipo.Credito;
                                bool esPrestamo = false;
                                bool esDeposito = false;
                                int idBancoDeposito = 0;
                                bool esPagoCredito = false;
                                int idTarjetaCredito = 0;
                                int idPeriodoCredito = 0;
                                DateTime? fechaPrestamo = null;
                                int? idTarjeta = null;
                                int? idCuenta = null;
                                int idBanco = 0;
                                int idPeriodo = 0;
                                bool procesado = false;
                                int idPersona = 1;
                                int? idCategoria = null;
                                #endregion

                                #region Credito
                                if (esCredito)
                                {
                                    mes = row.Cell((int)CreditoPos.Mes).Value.ToString().IsNullOrEmpty() ? 0 : Convert.ToInt32(row.Cell((int)CreditoPos.Mes).Value.ToString());
                                    banco = row.Cell((int)CreditoPos.Banco).Value.ToString();
                                    descripcion = row.Cell((int)CreditoPos.Descripcion).Value.ToString();
                                    solesDecimal = row.Cell((int)CreditoPos.Soles).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell((int)CreditoPos.Soles).Value.ToString());
                                    dolaresDecimal = row.Cell((int)CreditoPos.Dolares).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell((int)CreditoPos.Dolares).Value.ToString());
                                    cuotasBool = row.Cell((int)CreditoPos.EsCuotas).Value.ToString().IsNullOrEmpty() ? false : true;
                                    pertence = row.Cell((int)CreditoPos.Pertenece).Value.ToString();
                                    categoria = row.Cell((int)CreditoPos.Categoria).Value.ToString();
                                    descripcionConInfo = row.Cell(10).Value.ToString().IsNullOrEmpty() ? true : false;
                                    procesado = !row.Cell((int)CreditoPos.Procesado).Value.ToString().IsNullOrEmpty();

                                    if (procesado)
                                    {
                                        descripcion = row.Cell((int)CreditoPos.DescripcionOriginal).Value.ToString();
                                        descripcionConInfo = true;
                                    }

                                    if (!row.Cell((int)CreditoPos.Fecha).Value.ToString().IsNullOrEmpty())
                                    {
                                        fecha = row.Cell((int)CreditoPos.Fecha).Value;
                                    }

                                    if (solesDecimal != 0)
                                    {
                                        idMoneda = (int)Moneda.Soles;
                                        monto = solesDecimal;
                                    }
                                    else if (solesDecimal == 0 && dolaresDecimal != 0)
                                    {
                                        idMoneda = (int)Moneda.Dolares;
                                        monto = dolaresDecimal;
                                    }


                                }
                                #endregion
                                #region Debito
                                else
                                {
                                    idTipoMovimiento = (int)MovimientoTipo.Debito;

                                    if (!row.Cell(1).Value.ToString().IsNullOrEmpty())
                                    {
                                        fecha = row.Cell(1).Value;
                                    }

                                    banco = row.Cell(2).Value.ToString();
                                    monto = row.Cell(3).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(3).Value.ToString());
                                    descripcion = row.Cell(4).Value.ToString();
                                    categoria = row.Cell(5).Value.ToString();
                                    pertence = row.Cell(6).Value.ToString();
                                    idMoneda = row.Cell(7).Value.ToString().IsNullOrEmpty() ? 1 : Convert.ToInt32(row.Cell(7).Value.ToString());
                                    esPrestamo = row.Cell(8).Value.ToString().IsNullOrEmpty() ? false : true;

                                    if (!row.Cell(9).Value.ToString().IsNullOrEmpty())
                                    {
                                        fechaPrestamo = row.Cell(9).Value;
                                    }

                                    esDeposito = row.Cell(10).Value.ToString().IsNullOrEmpty() ? false : true;

                                    if (esDeposito)
                                    {
                                        int idBancoPago = lstBancos?.Where(x => x.abreviatura.ToUpper().Equals(row.Cell(10).Value.ToString().ToUpper())).FirstOrDefault()?.id ?? 0;
                                        idBancoDeposito = lstTarjetas?.Where(x => x.idBanco.Equals(idBancoPago) && x.idTipoTarjeta.Equals(1)).FirstOrDefault()?.id ?? 0;
                                    }

                                    esPagoCredito = row.Cell(11).Value.ToString().IsNullOrEmpty() ? false : true;

                                    if (esPagoCredito)
                                    {
                                        int idBancoPago = lstBancos?.Where(x => x.abreviatura.ToUpper().Equals(row.Cell(11).Value.ToString().ToUpper())).FirstOrDefault()?.id ?? 0;
                                        int idTarjetaPago = lstTarjetas?.Where(x => x.idBanco.Equals(idBancoPago) &&
                                                                                    x.idPropietario.Equals(idPropietario) &&
                                                                                    x.idTipoTarjeta.Equals(2)
                                                                                    ).FirstOrDefault()?.id ?? 0;

                                        int idMesCredito = row.Cell(12).Value.ToString().IsNullOrEmpty() ? 1 : Convert.ToInt32(row.Cell(12).Value.ToString());
                                        int idAnioCredito = row.Cell(13).Value.ToString().IsNullOrEmpty() ? 1 : Convert.ToInt32(row.Cell(13).Value.ToString());

                                        idPeriodoCredito = lstPeriodos?.Where(x => x.idTarjeta.Equals(idTarjetaPago) && x.anio.Equals(idAnioCredito) && x.mes.Equals(idMesCredito)).FirstOrDefault()?.id ?? 0;
                                    }

                                    cuotasBool = false;
                                    seguroBool = false;
                                    descripcionConInfo = false;
                                }
                                #endregion

                                #region idBanco, idTarjeta, idPeriodo
                                idBanco = lstBancos?.Where(x => x.abreviatura.ToUpper().Equals(banco.ToUpper())).FirstOrDefault()?.id ?? 0;

                                if (idTipoMovimiento == (int)MovimientoTipo.Credito)
                                {
                                    idTarjeta = lstTarjetas?.Where(x => x.idBanco.Equals(idBanco) && x.idPropietario.Equals(idPropietario)).FirstOrDefault()?.id;
                                }
                                else
                                {
                                    var idTarjetaCuentas = lstTarjetas?.Where(x => x.idTipoTarjeta.Equals(1) &&
                                                                            x.idBanco.Equals(idBanco) &&
                                                                            x.idPropietario.Equals(idPropietario)
                                                                        ).FirstOrDefault();
                                    if (idTarjetaCuentas is not null)
                                    {
                                        idCuenta = lstCuentas?.Where(x => x.idTarjeta.Equals(idTarjetaCuentas.id) && x.idMoneda.Equals(idMoneda)).FirstOrDefault()?.id;
                                    }
                                }

                                idPeriodo = lstPeriodos?.Where(x => x.idTarjeta.Equals(idTarjeta) &&
                                                                    x.anio.Equals(año) && x.mes.Equals(mes)
                                                                    ).FirstOrDefault()?.id ?? 0;
                                #endregion

                                #region Pertenece
                                if (!pertence.IsNullOrEmpty())
                                {
                                    var infoPertenece = pertence.Split(' ');
                                    var infoPersona = new tb_persona();

                                    if (infoPertenece.Length > 0)
                                        infoPersona = lstPersonas?.Where(x =>
                                                                    x.nombre1.Trim().ToUpper().Equals(infoPertenece[0].ToString().ToUpper()) &&
                                                                    x.apePaterno.Trim().ToUpper().Equals(infoPertenece[1].ToString().ToUpper())
                                                                    ).FirstOrDefault();
                                    else
                                        infoPersona = lstPersonas?.Where(x => x.nombre1.Trim().ToUpper().Equals(pertence.ToUpper())).FirstOrDefault();

                                    if (infoPersona is not null)
                                        idPersona = infoPersona.id;
                                }
                                else
                                {
                                    idPersona = idPropietario;
                                }
                                #endregion

                                #region Categoria
                                if (!categoria.IsNullOrEmpty())
                                {
                                    var infoCategoria = lstCategoria.Where(x => x.descripcion.Trim().ToUpper().Equals(categoria.ToUpper())).FirstOrDefault();

                                    if (infoCategoria is not null)
                                    {
                                        idCategoria = infoCategoria.id;
                                        seguroBool = idCategoriaSeguro == idCategoria;
                                    }
                                }
                                #endregion

                                var mov = new Movimiento()
                                {
                                    Mes = mes,
                                    Año = año,
                                    Linea = linea,
                                    Banco = banco,
                                    Descripcion = descripcion,
                                    DescripcionOriginal = descripcion,
                                    Soles = solesDecimal,
                                    Dolares = dolaresDecimal,
                                    Monto = monto,
                                    Cuotas = cuotasBool,
                                    Seguro = seguroBool,
                                    idMoneda = idMoneda,
                                    idTarjeta = idTarjeta,
                                    idPeriodo = idPeriodo,
                                    idCategoria = idCategoria,
                                    idPersona = idPersona,
                                    DescripcionConInfo = descripcionConInfo,
                                    idTipoMovimiento = idTipoMovimiento,
                                    esPrestamo = esPrestamo,
                                    esDeposito = esDeposito,
                                    idBancoDeposito = idBancoDeposito,
                                    esPagoCredito = esPagoCredito,
                                    idTarjetaCredito = idTarjetaCredito,
                                    FechaMovimiento = fecha,
                                    fechaPrestamo = fechaPrestamo,
                                    idCuenta = idCuenta,
                                    idPeriodoPago = idPeriodoCredito,
                                    Procesado = procesado,
                                    SheetName = workSheet.Name
                                };

                                #region Actualizar valores excel
                                if (!mov.Procesado && mov.idTipoMovimiento == (int)MovimientoTipo.Credito)
                                {
                                    var resultDescripcion = cuentasClass.ProcesarDescripcion(mov);

                                    var dec = resultDescripcion.FechaMovimiento;
                                    Console.WriteLine(dec.ToString());

                                    if (resultDescripcion.ResultProcesarDescripcion)
                                    {
                                        if (!resultDescripcion.Descripcion.IsNullOrEmpty())
                                            mov.Descripcion = resultDescripcion.Descripcion;

                                        workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, (int)CreditoPos.Descripcion).Value = resultDescripcion.Descripcion ?? "";

                                        if (!mov.Cuotas)
                                        {
                                            workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, (int)CreditoPos.Fecha).Value = resultDescripcion.FechaMovimiento;
                                        }
                                        else
                                        {
                                            workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, (int)CreditoPos.NroCuotas).Value = resultDescripcion.Cuota.nroCuotas;
                                            workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, (int)CreditoPos.NroCuota).Value = resultDescripcion.Cuota.nroCuota;
                                        }

                                        workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, (int)CreditoPos.DescripcionOriginal).Value = resultDescripcion.DescripcionOriginal ?? "";
                                    }

                                    workSheet.Cell(row.RangeAddress.FirstAddress.RowNumber, (int)CreditoPos.Procesado).Value = "X";
                                    workBook.Save();
                                }
                                #endregion

                                movimientos.Add(mov);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Linea: " + linea + " - " + ex.ToString());
                            }

                            linea++;
                        }
                    }
                }
            }

            return movimientos;
        }
    }
}
