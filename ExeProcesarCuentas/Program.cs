// See https://aka.ms/new-console-template for more information
using ClosedXML.Excel;
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
List<tb_moneda> lstMonedas = new List<tb_moneda>();
List<tb_pais> lstPaises = new List<tb_pais>();

var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
    .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
    .Options;

using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
{
    lstMonedas = cuentasDbContext.monedas.ToList();
    lstPaises = cuentasDbContext.paises.ToList();
}

//Read excel
//using var wbook = new XLWorkbook(excelFilePath);

//var ws1 = wbook.Worksheet(1);
//var data = ws1.Cell("A1").GetValue<string>();

//Console.WriteLine(data);

List<Movimiento> movimientos = new List<Movimiento>();

using (XLWorkbook workBook = new XLWorkbook(excelFilePath))
{
    foreach (IXLWorksheet workSheet in workBook.Worksheets)
    {
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
                int mes = row.Cell(1).Value.ToString().IsNullOrEmpty() ? 0 : Convert.ToInt32(row.Cell(1).Value.ToString());
                int año = row.Cell(2).Value.ToString().IsNullOrEmpty() ? 0 : Convert.ToInt32(row.Cell(2).Value.ToString());

                decimal solesDecimal = row.Cell(5).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(5).Value.ToString());
                decimal dolaresDecimal = row.Cell(6).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(6).Value.ToString());

                bool cuotasBool = row.Cell(7).Value.ToString().IsNullOrEmpty() ? false : true;
                bool seguroBool = row.Cell(8).Value.ToString().IsNullOrEmpty() ? false : true;

                movimientos.Add(new Movimiento()
                {
                    Mes = mes,
                    Año = año,
                    Linea = linea,
                    Banco = row.Cell(3).Value.ToString(),
                    Descripcion = row.Cell(4).Value.ToString(),
                    Soles = solesDecimal,
                    Dolares = dolaresDecimal,
                    Cuotas = cuotasBool,
                    Seguro = seguroBool
                });

                linea++;
            }
        }
    }
}

foreach (var mov in movimientos)
{
    var resultProces = cuentasClass.ProcesarDescripcion(mov);
    Console.WriteLine($"Mov: soles: {mov.Soles}, dolares: {mov.Dolares}");
}