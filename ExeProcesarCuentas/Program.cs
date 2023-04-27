// See https://aka.ms/new-console-template for more information
using ClosedXML.Excel;
using ExeProcesarCuentas;
using ExeProcesarCuentas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IO;

// load the configuration file.
var configBuilder = new ConfigurationBuilder().
   AddJsonFile("appsettings.json").Build();

var excelFilePath = configBuilder.GetSection("Paths:ExcelFilePath").Value;

//var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
//    .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
//    .Options;

//using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
//{
//    var persons = cuentasDbContext.monedas.ToList();

//    foreach (var person in persons)
//    {
//        Console.WriteLine($"Hello {person.descripcion}");
//    }
//}

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
        foreach (IXLRow row in workSheet.Rows())
        {
            if (firstRow)
            {
                firstRow = false;
            }
            else
            {
                decimal solesDecimal = row.Cell(3).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(3).Value.ToString());
                decimal dolaresDecimal = row.Cell(4).Value.ToString().IsNullOrEmpty() ? Convert.ToDecimal("0.00") : Convert.ToDecimal(row.Cell(4).Value.ToString());

                bool cuotasBool = row.Cell(5).Value.ToString().IsNullOrEmpty() ? false : true;
                bool seguroBool = row.Cell(6).Value.ToString().IsNullOrEmpty() ? false : true;

                movimientos.Add(new Movimiento()
                {
                    Banco = row.Cell(1).Value.ToString(),
                    Descripcion = row.Cell(2).Value.ToString(),
                    Soles = solesDecimal,
                    Dolares = dolaresDecimal,
                    Cuotas = cuotasBool,
                    Seguro = seguroBool
                });
            }
        }
    }
}

foreach (var mov in movimientos)
{
    Console.WriteLine($"Mov: soles: {mov.Soles}, dolares: {mov.Dolares}");
}