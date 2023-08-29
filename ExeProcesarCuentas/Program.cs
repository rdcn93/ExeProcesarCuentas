// See https://aka.ms/new-console-template for more information
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using ExeProcesarCuentas;
using ExeProcesarCuentas.Data;
using ExeProcesarCuentas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IO;

// load the configuration file.
var configBuilder = new ConfigurationBuilder().
   AddJsonFile("appsettings.json").Build();

var excelFilePath = configBuilder.GetSection("Paths:ExcelFilePath").Value;
Cuentas cuentasClass = new Cuentas();
List<tb_movimiento> newMovimientos = new List<tb_movimiento>();

List<tb_tarjeta> lstTarjetas = new List<tb_tarjeta>();
List<tb_moneda> lstMonedas = new List<tb_moneda>();
List<tb_banco> lstBancos = new List<tb_banco>();
List<tb_pais> lstPaises = new List<tb_pais>();
List<tb_categoria> lstCategoria = new List<tb_categoria>();
List<tb_persona> lstPersonas = new List<tb_persona>();
List<tb_tarjeta_periodo> lstPeriodos = new List<tb_tarjeta_periodo>();

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
}



List<Movimiento> movimientos = new List<Movimiento>();
bool registrar = false;
bool validarPeriodos = false;
bool validarExcel = false;
bool generarReporte = true;

if (validarPeriodos)
{
    bool resultValidacionPeriodos = cuentasClass.ValidarPeriodosTarjetas();

    //if (!resultValidacionPeriodos)
    //    Environment.Exit(0);
}

if (validarExcel)
{
    using (XLWorkbook workBook = new XLWorkbook(excelFilePath))
    {
        foreach (IXLWorksheet workSheet in workBook.Worksheets)
        {
            if (workSheet.Position != 1)
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
                        Console.WriteLine("Linea: " + linea);

                        int mes = row.Cell(1).Value.ToString().IsNullOrEmpty() ? 0 : Convert.ToInt32(row.Cell(1).Value.ToString());
                        int año = row.Cell(2).Value.ToString().IsNullOrEmpty() ? 0 : Convert.ToInt32(row.Cell(2).Value.ToString());
                        string banco = row.Cell(3).Value.ToString();

                        decimal solesDecimal = row.Cell(5).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(5).Value.ToString());
                        decimal dolaresDecimal = row.Cell(6).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(6).Value.ToString());

                        bool cuotasBool = row.Cell(7).Value.ToString().IsNullOrEmpty() ? false : true;
                        bool seguroBool = row.Cell(8).Value.ToString().IsNullOrEmpty() ? false : true;

                        string pertence = row.Cell(9).Value.ToString();
                        int idPersona = 1;

                        if (!pertence.IsNullOrEmpty())
                        {
                            var infoPersona = lstPersonas.Where(x => x.nombre1.Trim().ToUpper().Equals(pertence.ToUpper())).FirstOrDefault();

                            if (infoPersona is not null)
                                idPersona = infoPersona.id;
                        }
                        else
                        {
                            idPersona = lstPersonas?.Where(x => x.apePaterno.Trim().Equals("Castañeda") && x.nombre1.Trim().Equals("Raul"))?.FirstOrDefault()?.id ?? 0;
                        }

                        int idMoneda = 0;
                        int idBanco = lstBancos?.Where(x => x.abreviatura.ToUpper().Equals(banco.ToUpper())).FirstOrDefault()?.id ?? 0;
                        int idTarjeta = lstTarjetas?.Where(x => x.idBanco.Equals(idBanco)).FirstOrDefault()?.id ?? 0;
                        int idPeriodo = lstPeriodos?.Where(x => x.idTarjeta.Equals(idTarjeta) && x.anio.Equals(año) && x.mes.Equals(mes)).FirstOrDefault()?.id ?? 0;

                        if (solesDecimal != 0)
                        {
                            idMoneda = 1;
                        }
                        else if (solesDecimal == 0 && dolaresDecimal != 0)
                        {
                            idMoneda = 2;
                        }

                        movimientos.Add(new Movimiento()
                        {
                            Mes = mes,
                            Año = año,
                            Linea = linea,
                            Banco = banco,
                            Descripcion = row.Cell(4).Value.ToString(),
                            Soles = solesDecimal,
                            Dolares = dolaresDecimal,
                            Monto = idMoneda == 1 ? solesDecimal : dolaresDecimal,
                            Cuotas = cuotasBool,
                            Seguro = seguroBool,
                            idMoneda = idMoneda,
                            idTarjeta = idTarjeta,
                            idPeriodo = idPeriodo,
                            idPersona = idPersona
                        });
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
}

if (registrar)
{
    foreach (var mov in movimientos)
    {
        var result = cuentasClass.ProcesarDescripcion(mov);
        int cantMovMismoDia = movimientos.Where(x => x.FechaMovimiento.Equals(mov.FechaMovimiento) && 
        x.Descripcion.Equals(mov.Descripcion) && 
        x.Monto.Equals(mov.Monto) &&
        x.idTarjeta.Equals(mov.idTarjeta)
        ).Count();

        if (mov.Cuotas)
        {
            var fd = cuentasClass.RegistrarMovimientoEnCuotas(mov);
        }
        else
        {
            var dfd = cuentasClass.RegistrarMovimiento(mov, false, cantMovMismoDia);
        }
    }

    var resultCompletarCuotas = cuentasClass.CompletarCuotasSinRegistrar();
}

if (generarReporte)
{
    cuentasClass.GenerarReporte();
}
