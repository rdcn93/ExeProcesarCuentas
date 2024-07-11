// See https://aka.ms/new-console-template for more information
using DocumentFormat.OpenXml;
using ExeProcesarCuentas.Classes;
using ExeProcesarCuentas.Data;
using ExeProcesarCuentas.Logic;
using ExeProcesarCuentas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using static ExeProcesarCuentas.Util.Enums;

// load the configuration file.
var configBuilder = new ConfigurationBuilder().
   AddJsonFile("appsettings.json").Build();

Cuentas cuentasClass = new Cuentas();
Excel excelClass = new Excel();

List<tb_movimiento> currentMovimientos = new List<tb_movimiento>();

var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
    .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
    .Options;

using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
{
    currentMovimientos = cuentasDbContext.movimientos.ToList();
}

List<Movimiento> movimientos = new List<Movimiento>();
bool validarPeriodos = false;
bool validarExcel = true;
bool registrar = true;
bool eliminar = false;
bool validarCuotasSinRegistrar = true;
bool generarReporte = false;
bool resetearValores = false;

if (validarPeriodos)
{
    bool resultValidacionPeriodos = cuentasClass.ValidarPeriodosTarjetas();

    //if (!resultValidacionPeriodos)
    //    Environment.Exit(0);
}

if (resetearValores)
{
    var result = excelClass.ResetearValoresExcelCompleto();
}

if (validarExcel)
{
    movimientos = excelClass.ObtenerMovimientosExcelCompleto();
}

if (eliminar)
{
    foreach (var mov in currentMovimientos)
    {
        var eliminarM = false;

        if (mov.idMovimientoTipo == (int)MovimientoTipo.Credito)
        {
            eliminarM = movimientos.Where(x => x.FechaMovimiento.Equals(mov.fecha) &&
                                                x.Descripcion.Equals(mov.descripcion) &&
                                                x.Monto == mov.monto &&
                                                x.idTarjeta.Equals(mov.idTarjeta) &&
                                                x.idTipoMovimiento.Equals((int)MovimientoTipo.Credito)
                                                ).Any();
        }
        else if (mov.idMovimientoTipo == (int)MovimientoTipo.Debito)
        {
            //eliminar = movimientos.Where(x => x.FechaMovimiento.Equals(mov.fecha) &&
            //                                    x.Monto == mov.monto &&
            //                                    x.idCuenta.Equals(mov.idCuenta) &&
            //                                    x.idTipoMovimiento.Equals((int)Util.MovimientoTipo.Debito)
            //                                    ).Any();
        }

        if (!eliminarM)
        {
            var idMov = mov.id;
            var resultD = cuentasClass.EliminarMovimientoFull(idMov);
        }
    }
}

if (registrar)
{
    foreach (var mov in movimientos)
    {
        //if (mov.DescripcionConInfo)
        //{
        //    var result = cuentasClass.ProcesarDescripcion(mov);
        //}else if(!mov.DescripcionConInfo && mov.idTipoMovimiento == (int)Util.MovimientoTipo.Credito)
        //{
        //    var dsd = "";
        //}

        //bool eliminar = false;

        //var eliminado = currentMovimientos.Where(x => x.fecha.Equals(mov.FechaMovimiento) &&
        //                                        x.descripcion.Equals(mov.Descripcion) &&
        //                                        x.monto == mov.Monto &&
        //                                        x.idTarjeta.Equals(mov.idTarjeta) &&
        //                                        x.idMovimientoTipo.Equals((int)Util.MovimientoTipo.Credito)
        //                                        ).Any();

        //if (!eliminado)
        //{
        //    var idMov = mov.Id;
        //    var resultD = cuentasClass.EliminarMovimientoFull(idMov);
        //}

        int cantMovMismoDia = 0;

        if (mov.idTipoMovimiento == (int)MovimientoTipo.Credito)
        {
            cantMovMismoDia = movimientos.Where(x => x.FechaMovimiento.Equals(mov.FechaMovimiento) &&
                                                //x.Descripcion.Equals(mov.Descripcion) &&
                                                x.Monto == mov.Monto &&
                                                x.idTarjeta.Equals(mov.idTarjeta) &&
                                                x.idTipoMovimiento.Equals((int)MovimientoTipo.Credito)
                                                ).Count();
        }
        else if (mov.idTipoMovimiento == (int)MovimientoTipo.Debito)
        {
            cantMovMismoDia = movimientos.Where(x => x.FechaMovimiento.Equals(mov.FechaMovimiento) &&
                                                x.Monto == mov.Monto &&
                                                x.idCuenta.Equals(mov.idCuenta) &&
                                                x.idPersona.Equals(mov.idPersona) && 
                                                x.idTipoMovimiento.Equals((int)MovimientoTipo.Debito)
                                                ).Count();
        }

        if (mov.Cuotas)
        {
            var resultRegistrarMovCuotas = cuentasClass.RegistrarMovimientoEnCuotas(mov);
        }
        else
        {
            var resultRegistrarMov = cuentasClass.RegistrarMovimiento(mov, false, cantMovMismoDia);

            if (mov.Id == 0 && resultRegistrarMov != 0)
                mov.Id = resultRegistrarMov;
        }

        if (mov.esPrestamo)
        {
            cuentasClass.ValidarPrestamo(mov);
        }

        if (mov.esPagoCredito)
        {
            cuentasClass.ValidarPeriodoPago(mov);
        }
    }

    if (validarCuotasSinRegistrar)
    {
        var resultCompletarCuotas = cuentasClass.CompletarCuotasSinRegistrar();
    }
}

if (generarReporte)
{
    cuentasClass.GenerarReporte();
}
