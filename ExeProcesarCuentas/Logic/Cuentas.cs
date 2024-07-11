using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using ExeProcesarCuentas.Classes;
using ExeProcesarCuentas.Data;
using ExeProcesarCuentas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using ExeProcesarCuentas.Util;
using DocumentFormat.OpenXml.InkML;

namespace ExeProcesarCuentas.Logic
{
    internal class Cuentas
    {
        private int idTarjeta = 0;
        private int[] años = new int[] { 2022, 2023, 2024, 2025 };

        public Movimiento ProcesarDescripcion(Movimiento mov)
        {
            if (!mov.Banco.IsNullOrEmpty() && !mov.Descripcion.IsNullOrEmpty())
            {
                var lstDescripcion = mov.Descripcion.Split(' ').ToList();
                int cantBorrarInicioDescripcion = 0;
                int cantBorrarFinalDescripcion = 0;

                string dateFormat = "";
                string dateDescription = "";
                string dateMovimiento = "";

                #region Variables Movimiento
                string strDescripcion = "";
                DateTime fechaMovimiento = DateTime.Today;
                DateTime fechaProcesamiento = DateTime.Today;
                int nroCuotas = 0;
                int nroCuota = 0;
                int nroCuotasPorPagar = 0;
                decimal TEA = 0;
                decimal monto = 0;
                decimal montoCuota = 0;
                decimal montoPorPagar = 0;
                decimal capital = 0;
                decimal intereses = 0;
                #endregion

                try
                {
                    #region Normal
                    if (!mov.Cuotas && !mov.Seguro)
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
                                dateMovimiento = lstDescripcion[2] + "-" + MesADatetimeFormat(lstDescripcion[3]);
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
                    }
                    #endregion
                    #region Cuotas
                    else if (mov.Cuotas)
                    {
                        switch (mov.Banco.ToUpper())
                        {
                            case "INTERBANK":
                                nroCuotas = int.Parse(lstDescripcion[lstDescripcion.Count - 9]);
                                nroCuotasPorPagar = int.Parse(lstDescripcion[lstDescripcion.Count - 8]);
                                nroCuota = nroCuotas - nroCuotasPorPagar;
                                TEA = decimal.Parse(lstDescripcion[lstDescripcion.Count - 7]);
                                monto = decimal.Parse(lstDescripcion[lstDescripcion.Count - 6]);
                                montoPorPagar = decimal.Parse(lstDescripcion[lstDescripcion.Count - 5]);
                                capital = decimal.Parse(lstDescripcion[lstDescripcion.Count - 4]);
                                intereses = decimal.Parse(lstDescripcion[lstDescripcion.Count - 3]);
                                montoCuota = decimal.Parse(lstDescripcion[lstDescripcion.Count - (mov.idMoneda == 1 ? 2 : 1)]);

                                var lstFecInterbank = lstDescripcion[0].Split('-');
                                dateDescription = lstFecInterbank[0] + "-" + MesADatetimeFormat(lstFecInterbank[1]);
                                dateFormat = "dd-MMM";
                                cantBorrarInicioDescripcion = 1;
                                cantBorrarFinalDescripcion = 9;
                                break;
                            case "DINERS":
                                var lstCuotasDiners = lstDescripcion[lstDescripcion.Count - 7].Replace("(", "").Replace(")", "").Split('/');
                                nroCuotas = int.Parse(lstCuotasDiners[1]);
                                nroCuota = int.Parse(lstCuotasDiners[0]);
                                nroCuotasPorPagar = nroCuotas - nroCuota;

                                TEA = decimal.Parse(lstDescripcion[lstDescripcion.Count - 6].Replace("%", ""));
                                monto = decimal.Parse(lstDescripcion[lstDescripcion.Count - 5]);
                                montoPorPagar = decimal.Parse(lstDescripcion[lstDescripcion.Count - 4]);
                                capital = decimal.Parse(lstDescripcion[lstDescripcion.Count - 3]);
                                intereses = decimal.Parse(lstDescripcion[lstDescripcion.Count - 2]);
                                montoCuota = decimal.Parse(lstDescripcion[lstDescripcion.Count - 1]);

                                dateDescription = lstDescripcion[0] + "-" + MesADatetimeFormat(lstDescripcion[1]);
                                dateFormat = "dd-MMM";
                                cantBorrarInicioDescripcion = 4;
                                cantBorrarFinalDescripcion = 7;
                                break;
                            case "BCP":
                                dateDescription = lstDescripcion[0] + ".";
                                dateFormat = "ddMMM";
                                cantBorrarInicioDescripcion = 2;
                                cantBorrarFinalDescripcion = 1;
                                break;
                            case "BBVA":
                                nroCuotas = int.Parse(lstDescripcion[lstDescripcion.Count - 6]);
                                nroCuota = int.Parse(lstDescripcion[lstDescripcion.Count - 8]);
                                nroCuotasPorPagar = nroCuotas - nroCuota;
                                TEA = decimal.Parse(lstDescripcion[lstDescripcion.Count - 5].Replace("%", ""));
                                monto = decimal.Parse(lstDescripcion[lstDescripcion.Count - 9]);
                                capital = decimal.Parse(lstDescripcion[lstDescripcion.Count - 4]);
                                intereses = decimal.Parse(lstDescripcion[lstDescripcion.Count - 3]);
                                montoCuota = decimal.Parse(lstDescripcion[lstDescripcion.Count - (mov.idMoneda == 1 ? 2 : 1)]);

                                dateDescription = lstDescripcion[0];
                                dateFormat = "dd-MM-yyyy";
                                cantBorrarInicioDescripcion = 1;
                                cantBorrarFinalDescripcion = 9;
                                break;
                        }
                    }
                    #endregion
                    #region Seguro
                    else if (mov.Seguro)
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
                                cantBorrarFinalDescripcion = 1;
                                break;
                            case "BCP":
                                dateDescription = "26-" + (mov.Mes < 10 ? "0" : "") + mov.Mes.ToString() + "-" + mov.Año.ToString();
                                dateFormat = "dd-MM-yyyy";
                                cantBorrarInicioDescripcion = 2;
                                cantBorrarFinalDescripcion = 1;
                                break;
                            case "BBVA":
                                dateDescription = lstDescripcion[0];
                                dateFormat = "dd-MM";
                                cantBorrarInicioDescripcion = 1;
                                cantBorrarFinalDescripcion = 2;
                                break;
                        }
                    }
                    #endregion

                    #region Retirar valores array de descripcion
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
                    #endregion

                    dateDescription = dateDescription.Replace("Sep", "Set");

                    if (!dateFormat.Contains("yyyy"))
                    {
                        int anio = mov.Año;

                        if (mov.Cuotas)
                        {
                            var fechaMovCuotas = DateTime.ParseExact(dateDescription + "-" + mov.Año, dateFormat + "-yyyy", CultureInfo.CreateSpecificCulture("es-PE"));
                            if (nroCuota > 1)
                            {
                                for (int i = 1; i <= nroCuota; i++)
                                {
                                    fechaMovCuotas = fechaMovCuotas.AddMonths(-1);
                                }
                            }

                            anio = fechaMovCuotas.Year;
                        }

                        dateFormat = dateFormat + "-yyyy";
                        dateDescription = dateDescription + "-" + anio.ToString();
                    }

                    fechaMovimiento = DateTime.ParseExact(dateDescription, dateFormat, CultureInfo.CreateSpecificCulture("es-PE"));


                    if (fechaMovimiento.Month == 12)
                    {
                        if (mov.Mes == 1)
                            fechaMovimiento = fechaMovimiento.AddYears(-1);
                        else if (mov.Mes > 1)
                        {
                            int days = DateTime.DaysInMonth(mov.Año, mov.Mes);
                            int diaMov = fechaMovimiento.Day;
                            if (fechaMovimiento.Day > days)
                            {
                                bool lasDayMonth = false;

                                do
                                {
                                    diaMov--;

                                    if (days == diaMov)
                                        lasDayMonth = true;
                                } while (!lasDayMonth);
                            }

                            DateTime fechaMovimientoCuota = new DateTime(mov.Año, mov.Mes, diaMov);
                            if (nroCuota > 1)
                            {
                                fechaMovimientoCuota = fechaMovimientoCuota.AddMonths(-nroCuota);

                                if (fechaMovimiento.Year != fechaMovimientoCuota.Year)
                                {
                                    int diffYears = fechaMovimientoCuota.Year - fechaMovimiento.Year;

                                    fechaMovimiento = fechaMovimiento.AddYears(diffYears);
                                }
                            }
                        }
                    }

                    strDescripcion = string.Join(" ", lstDescripcion.ToArray());

                    mov.Descripcion = strDescripcion;
                    mov.FechaMovimiento = fechaMovimiento;

                    if (mov.Cuotas)
                    {
                        mov.Monto = monto;

                        mov.Cuota = new MovimientoCuota()
                        {
                            nroCuotas = nroCuotas,
                            nroCuota = nroCuota,
                            nroCuotasPorPagar = nroCuotasPorPagar,
                            TEA = TEA,
                            monto = monto,
                            montoCuota = montoCuota,
                            montoPorPagar = montoPorPagar,
                            capital = capital,
                            intereses = intereses,
                        };
                    }

                    mov.ResultProcesarDescripcion = true;
                }
                catch (Exception ex)
                {
                    mov.DescripcionConInfo = false;
                    Console.WriteLine($"Name: {mov.SheetName}, Linea: {mov.Linea}, Banco: {mov.Banco}, Error {ex.Message}");
                }
            }

            return mov;
        }

        public bool ValidarPeriodosTarjetas()
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            List<tb_periodo_configuracion> lstPeriodosConfiguracion = new List<tb_periodo_configuracion>();
            List<tb_banco> lstBancos = new List<tb_banco>();
            List<tb_tarjeta> lstTarjetas = new List<tb_tarjeta>();
            List<tb_tarjeta_periodo> lstPeriodos = new List<tb_tarjeta_periodo>();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                lstPeriodosConfiguracion = cuentasDbContext.periodosConfiguracion.ToList();
                lstBancos = cuentasDbContext.bancos.ToList();
                lstTarjetas = cuentasDbContext.tarjetas.ToList();
                lstPeriodos = cuentasDbContext.periodos.ToList();
            }


            List<tb_tarjeta_periodo> nuevosPeriodos = new List<tb_tarjeta_periodo>();

            foreach (var a in años)
            {
                for (int i = 1; i <= 12; i++)
                {
                    foreach (var b in lstTarjetas)
                    {
                        DateTime primerDiaMes = new DateTime(a, i, 1);
                        var periodoConf = lstPeriodosConfiguracion.Where(x => x.idTarjeta.Equals(b.id)).FirstOrDefault();

                        if (periodoConf != null)
                        {
                            var fechaInicio = ObtenerDiaInicioPeriodo(a, i, periodoConf.diaInicio, periodoConf.diaCorte, periodoConf.corteDiaHabil);
                            var fechaCorte = ObtenerDiaCortePeriodo(a, i, periodoConf.diaCorte, periodoConf.corteDiaHabil);

                            Console.WriteLine(primerDiaMes.ToString("yyyy-MM") + " " + fechaInicio.ToString("dd/MM/yyyy") + " - " + fechaCorte.ToString("dd/MM/yyyy"));

                            var periodosMesAnioTarjeta = lstPeriodos?.Where(x => x.mes.Equals(i) && x.anio.Equals(a) && x.idTarjeta.Equals(b.id)).FirstOrDefault();

                            tb_tarjeta_periodo tarjetaPeriodo = new tb_tarjeta_periodo
                            {
                                idTarjeta = b.id,
                                mes = i,
                                anio = a,
                                fechaInicio = fechaInicio,
                                fechaCorte = fechaCorte,
                                fechaMinPago = fechaCorte.AddDays(1)
                            };

                            if (periodosMesAnioTarjeta == null)
                            {

                                using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
                                {
                                    try
                                    {
                                        cuentasDbContext.periodos.Add(tarjetaPeriodo);

                                        cuentasDbContext.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
                                {
                                    try
                                    {
                                        if (tarjetaPeriodo != null)
                                        {
                                            tarjetaPeriodo.id = periodosMesAnioTarjeta.id;

                                            cuentasDbContext.periodos.Attach(tarjetaPeriodo);
                                            cuentasDbContext.periodos.Update(tarjetaPeriodo);

                                            cuentasDbContext.SaveChanges();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public int RegistrarMovimientoEnCuotas(Movimiento mov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            if (mov.Descripcion.Equals("IMPORTACIONES H"))
            {
                string des = mov.Descripcion;
            }

            int idMovimiento = 0;

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                var movimientoInfo = ObtenerMovimiento(mov);

                if (movimientoInfo is null)
                    idMovimiento = RegistrarMovimiento(mov, true);
                else
                    idMovimiento = movimientoInfo.id;
            }

            var newMovCuotas = new tb_movimiento_cuota()
            {
                idMovimiento = idMovimiento,
                nroCuota = mov.Cuota.nroCuota,
                nroCuotas = mov.Cuota.nroCuotas,
                nroCuotasPorPagar = mov.Cuota.nroCuotasPorPagar,
                TEA = mov.Cuota.TEA,
                monto = mov.Cuota.monto,
                montoCuota = mov.Cuota.montoCuota,
                montoPorPagar = mov.Cuota.montoPorPagar,
                capital = mov.Cuota.capital,
                intereses = mov.Cuota.intereses,
                idPeriodoCuota = mov.idPeriodo
            };

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movCuota = cuentasDbContext.movimientoCuotas.Where(x => x.idMovimiento.Equals(idMovimiento) &&
                                                                           x.nroCuota.Equals(newMovCuotas.nroCuota) &&
                                                                           x.nroCuotas.Equals(newMovCuotas.nroCuotas) &&
                                                                           x.idPeriodoCuota.Equals(newMovCuotas.idPeriodoCuota)
                                                ).FirstOrDefault();



                    if (movCuota is null)
                    {
                        RegistrarMovimientoCuota(newMovCuotas);
                    }
                    else
                    {
                        newMovCuotas.id = movCuota.id;
                        UpdateMovimientoCuota(newMovCuotas);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return 0;
        }

        public bool RegistrarMovimientoCuota(tb_movimiento_cuota newMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    cuentasDbContext.movimientoCuotas.Add(newMov);
                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool UpdateMovimientoCuota(tb_movimiento_cuota newMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movCuota = cuentasDbContext.movimientoCuotas.Find(newMov.id);

                    movCuota.montoPorPagar = newMov.montoPorPagar;
                    movCuota.capital = newMov.capital;
                    movCuota.intereses = newMov.intereses;
                    movCuota.montoCuota = newMov.montoCuota;

                    cuentasDbContext.movimientoCuotas.Attach(movCuota);
                    cuentasDbContext.movimientoCuotas.Update(movCuota);
                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool UpdateMovimiento(tb_movimiento newMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var mov = cuentasDbContext.movimientos.Find(newMov.id);

                    mov.descripcion = newMov.descripcion;
                    mov.idCategoria = newMov.idCategoria;
                    mov.idBeneficiario = newMov.idBeneficiario;
                    mov.idMovimientoTipo = newMov.idMovimientoTipo;

                    cuentasDbContext.movimientos.Attach(mov);
                    cuentasDbContext.movimientos.Update(mov);
                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public int RegistrarMovimiento(Movimiento mov, bool registroDeCuotas = false, int? cantidadDia = null)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            int idMovimiento = 0;
            var newMov = new tb_movimiento()
            {
                descripcion = mov.Descripcion,
                fecha = mov.FechaMovimiento,
                monto = mov.Monto,
                enCuotas = mov.Cuotas,
                nroCuotas = mov.Cuota?.nroCuotas ?? 1,
                idTarjeta = mov.idTarjeta,
                idMoneda = mov.idMoneda,
                idPais = null,
                idCategoria = mov.idCategoria,
                idBeneficiario = mov.idPersona,
                idPeriodo = mov.idPeriodo,
                idCuenta = mov.idCuenta,
                esSeguro = mov.Seguro,
                idMovimientoTipo = mov.idTipoMovimiento,
                estado = 1
            };

            bool registrar = false;
            bool actualizar = false;
            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                if (registroDeCuotas)
                {
                    var movimientoInfo = ObtenerMovimiento(mov);
                    if (movimientoInfo == null)
                        registrar = true;
                    else
                        idMovimiento = movimientoInfo.id;
                }
                else
                {
                    List<tb_movimiento> movimientosDia = new List<tb_movimiento>();

                    if (mov.idTipoMovimiento == (int)Enums.MovimientoTipo.Credito)
                    {
                        movimientosDia = cuentasDbContext.movimientos.Where(x =>
                                                                     x.fecha.Equals(mov.FechaMovimiento) &&
                                                                     x.monto.Equals(mov.Monto) &&
                                                                     x.idTarjeta.Equals(mov.idTarjeta) &&
                                                                     x.idMovimientoTipo.Equals((int)Enums.MovimientoTipo.Credito)
                                                                    //x.idPeriodo.Equals(mov.idPeriodo)
                                                                    ).ToList();
                    }
                    else
                    {
                        movimientosDia = cuentasDbContext.movimientos.Where(x =>
                                                                     x.fecha.Equals(mov.FechaMovimiento) &&
                                                                     x.monto.Equals(mov.Monto) &&
                                                                     x.idCuenta.Equals(mov.idCuenta) &&
                                                                     x.idBeneficiario.Equals(mov.idPersona) &&
                                                                     x.idMovimientoTipo.Equals((int)Enums.MovimientoTipo.Debito)
                                                                    ).ToList();
                    }

                    if (movimientosDia.Count < cantidadDia)
                        registrar = true;

                    if (movimientosDia.Count == 1 && !registrar)
                    {
                        newMov.id = movimientosDia.FirstOrDefault().id;
                        idMovimiento = newMov.id;
                        UpdateMovimiento(newMov);
                    }
                }

                if (registrar)
                {
                    try
                    {
                        cuentasDbContext.movimientos.Add(newMov);
                        cuentasDbContext.SaveChanges();

                        idMovimiento = newMov.id;
                        mov.Id = idMovimiento;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                else if (actualizar)
                {

                }
            }

            return idMovimiento;
        }

        public tb_movimiento ObtenerMovimiento(Movimiento mov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            tb_movimiento? movimientoInfo = null;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                if (mov.idTipoMovimiento == 1)
                {
                    movimientoInfo = cuentasDbContext.movimientos.Where(x => x.fecha.Equals(mov.FechaMovimiento) &&
                                                                     //x.descripcion.Equals(mov.Descripcion) &&
                                                                     x.monto.Equals(mov.Monto) &&
                                                                     x.idTarjeta.Equals(mov.idTarjeta) &&
                                                                     //x.idPeriodo.Equals(mov.idPeriodo) &&
                                                                     x.enCuotas.Equals(mov.Cuotas)
                                                                    ).FirstOrDefault();
                }
                else
                {
                    movimientoInfo = cuentasDbContext.movimientos.Where(x => x.fecha.Equals(mov.FechaMovimiento) &&
                                                                     x.monto.Equals(mov.Monto) &&
                                                                     x.idCuenta.Equals(mov.idCuenta) &&
                                                                     x.idBeneficiario == mov.idPersona
                                                                    ).FirstOrDefault();
                }

            }

            return movimientoInfo;
        }

        #region
        public bool EliminarMovimientoFull(int idMov)
        {
            bool result1 = EliminarMovimientoPrestamoDetalle(idMov);
            bool result2 = EliminarMovimientoPrestamoPago(idMov);
            bool result3 = EliminarMovimientoCuota(idMov);
            bool result4 = EliminarMovimientoTarjetaPago(idMov);
            bool result5 = EliminarMovimiento(idMov);

            return false;
        }

        public bool EliminarMovimientoPrestamoDetalle(int idMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movs = cuentasDbContext.prestamoDetalles.Where(x => x.idMovimiento.Equals(idMov)).ToList();

                    foreach (var m in movs)
                    {
                        cuentasDbContext.prestamoDetalles.Remove(m);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool EliminarMovimientoPrestamoPago(int idMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movs = cuentasDbContext.prestamoPagos.Where(x => x.idMovimiento.Equals(idMov)).ToList();

                    foreach (var m in movs)
                    {
                        cuentasDbContext.prestamoPagos.Remove(m);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool EliminarMovimientoCuota(int idMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movs = cuentasDbContext.movimientoCuotas.Where(x => x.idMovimiento.Equals(idMov)).ToList();

                    foreach (var m in movs)
                    {
                        cuentasDbContext.movimientoCuotas.Remove(m);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool EliminarMovimientoTarjetaPago(int idMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movs = cuentasDbContext.periodoPagos.Where(x => x.idMovimiento.Equals(idMov)).ToList();

                    foreach (var m in movs)
                    {
                        cuentasDbContext.periodoPagos.Remove(m);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool EliminarMovimiento(int idMov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var movs = cuentasDbContext.movimientos.Where(x => x.id.Equals(idMov)).ToList();

                    foreach (var m in movs)
                    {
                        cuentasDbContext.movimientos.Remove(m);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }
        #endregion

        #region Prestamo
        public void ValidarPrestamo(Movimiento mov)
        {
            var prestamo = ObtenerPrestamo(mov);
            var movimiento = ObtenerMovimiento(mov);
            var pretamoId = 0;
            if (prestamo is null)
            {
                var newPrestamo = new tb_prestamo();
                pretamoId = RegistrarPrestamo(mov);
            }
            else
            {
                pretamoId = prestamo.id;
                var resultUpdtPrestamo = ActualizarPrestamo(prestamo);
            }

            if (mov.fechaPrestamo is null)
            {
                RegistrarPrestamoDetalle(pretamoId, movimiento.id);
            }
            else
            {
                RegistrarPrestamoPago(pretamoId, movimiento.id);
            }
        }

        public tb_prestamo ObtenerPrestamo(Movimiento mov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            tb_prestamo? result = null;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    result = cuentasDbContext.prestamos.Where(x => x.fecha.Equals(mov.FechaMovimiento) &&
                                                    x.idCuenta.Equals(mov.idCuenta) &&
                                                    x.idBeneficiario.Equals(mov.idPersona)
                    ).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public int RegistrarPrestamo(Movimiento mov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            int newId = 0;
            var newPrestamo = new tb_prestamo()
            {
                fecha = mov.FechaMovimiento,
                descripcion = "",
                idCuenta = mov.idCuenta ?? 0,
                idBeneficiario = mov.idPersona,
                estado = 1
            };

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    cuentasDbContext.prestamos.Add(newPrestamo);
                    cuentasDbContext.SaveChanges();

                    newId = newPrestamo.id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return newId;
        }

        public bool ActualizarPrestamo(tb_prestamo prestamo)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var mov = cuentasDbContext.prestamos.Find(prestamo.id);

                    mov.descripcion = prestamo.descripcion;
                    mov.estado = prestamo.estado;

                    cuentasDbContext.prestamos.Attach(mov);
                    cuentasDbContext.prestamos.Update(mov);
                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public void RegistrarPrestamoDetalle(int idPrestamo, int idMovimiento, int? estado = null)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            int newId = 0;
            var newPrestamo = new tb_prestamo_detalle()
            {
                idPrestamo = idPrestamo,
                idMovimiento = idMovimiento,
                estado = 1,
            };

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var existePrestamoDetalle = cuentasDbContext.prestamoDetalles.Where(x => x.idPrestamo.Equals(idPrestamo) && x.idMovimiento.Equals(idMovimiento)).FirstOrDefault();

                    if (existePrestamoDetalle is null)
                    {
                        cuentasDbContext.prestamoDetalles.Add(newPrestamo);
                    }
                    else
                    {
                        existePrestamoDetalle.estado = estado != null ? (int)estado : 1;
                        cuentasDbContext.prestamoDetalles.Attach(existePrestamoDetalle);
                        cuentasDbContext.prestamoDetalles.Update(existePrestamoDetalle);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void RegistrarPrestamoPago(int idPrestamo, int idMovimiento, int? estado = null)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            int newId = 0;
            var newPrestamo = new tb_prestamo_pago()
            {
                idPrestamo = idPrestamo,
                idMovimiento = idMovimiento,
                estado = 1,
            };

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var existePrestamoDetalle = cuentasDbContext.prestamoPagos.Where(x => x.idPrestamo.Equals(idPrestamo) && x.idMovimiento.Equals(idMovimiento)).FirstOrDefault();

                    if (existePrestamoDetalle is null)
                    {
                        cuentasDbContext.prestamoPagos.Add(newPrestamo);
                    }
                    else
                    {
                        existePrestamoDetalle.estado = estado != null ? (int)estado : 1;
                        cuentasDbContext.prestamoPagos.Attach(existePrestamoDetalle);
                        cuentasDbContext.prestamoPagos.Update(existePrestamoDetalle);
                    }

                    cuentasDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region Periodo Pago
        public void ValidarPeriodoPago(Movimiento mov)
        {
            var pago = ObtenerPeriodoPago(mov.idPeriodoPago, mov.Id);
            var movimiento = ObtenerMovimiento(mov);

            if (pago is null)
            {
                RegistrarPeriodoPago(mov);
            }
        }

        public tb_tarjeta_periodo_pago ObtenerPeriodoPago(int idPeriodo, int idMovimiento)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            tb_tarjeta_periodo_pago? result = null;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    result = cuentasDbContext.periodoPagos.Where(x => x.idPeriodo.Equals(idPeriodo) &&
                                                    x.idMovimiento.Equals(idMovimiento)
                    ).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public bool RegistrarPeriodoPago(Movimiento mov)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;
            var newPago = new tb_tarjeta_periodo_pago()
            {
                idPeriodo = mov.idPeriodoPago,
                idMovimiento = mov.Id,
                monto = mov.Monto
            };

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                try
                {
                    var existePrestamoDetalle = cuentasDbContext.periodoPagos.Where(x => x.idPeriodo.Equals(mov.idPeriodoPago) &&
                                                                x.idMovimiento.Equals(mov.Id)).FirstOrDefault();

                    if (existePrestamoDetalle is null)
                    {
                        cuentasDbContext.periodoPagos.Add(newPago);
                    }
                    else
                    {
                        cuentasDbContext.periodoPagos.Attach(existePrestamoDetalle);
                        cuentasDbContext.periodoPagos.Update(existePrestamoDetalle);
                    }

                    cuentasDbContext.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }
        #endregion

        public bool CompletarCuotasSinRegistrar()
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            bool result = false;

            List<tb_tarjeta_periodo> periodos = new List<tb_tarjeta_periodo>();

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                periodos = cuentasDbContext.periodos.ToList();
                List<tb_movimiento> movimientos = cuentasDbContext.movimientos.ToList();
                List<tb_movimiento_cuota> movCuotas = cuentasDbContext.movimientoCuotas.ToList();

                List<int> idMovCoutas = movCuotas.Select(x => x.idMovimiento).Distinct().ToList();

                foreach (var idMov in idMovCoutas)
                {
                    var infoMov = movimientos.Where(x => x.id.Equals(idMov)).FirstOrDefault();
                    var MovCuotasList = movCuotas.Where(x => x.idMovimiento.Equals(idMov)).ToList();

                    var tea = MovCuotasList.FirstOrDefault()?.TEA ?? 0;
                    var montoCuota = MovCuotasList.FirstOrDefault()?.montoCuota ?? 0;

                    if (infoMov is not null)
                    {
                        for (int i = 1; i <= infoMov.nroCuotas; i++)
                        {
                            var infoMovCuota = movCuotas.Where(x => x.idMovimiento.Equals(idMov) && x.nroCuota.Equals(i)).FirstOrDefault();

                            if (infoMovCuota is null)
                            {
                                int idUltimoPeriodo = 0;
                                int diferenciaPeriodos = 0;

                                if (i == 1)
                                {
                                    int posicionSiguientePeriodo = i + 1;

                                    do
                                    {
                                        var infoSiguientePerido = MovCuotasList.Where(x => x.nroCuota.Equals(posicionSiguientePeriodo)).FirstOrDefault();

                                        if (infoSiguientePerido is not null)
                                        {
                                            idUltimoPeriodo = infoSiguientePerido.idPeriodoCuota;
                                        }
                                        else
                                        {
                                            posicionSiguientePeriodo++;
                                            diferenciaPeriodos++;
                                        }

                                    } while (idUltimoPeriodo == 0);

                                    for (int dif = 0; dif <= diferenciaPeriodos; dif++)
                                    {
                                        idUltimoPeriodo = ObtenerPeriodoSegunPeriodo(infoMov.idTarjeta, idUltimoPeriodo, true);
                                    }
                                }
                                else if (i == infoMov.nroCuotas)
                                {
                                    int posicionAnteriorPeriodo = i - 1;

                                    do
                                    {
                                        var infoAnteriorPerido = MovCuotasList.Where(x => x.nroCuota.Equals(posicionAnteriorPeriodo)).FirstOrDefault();

                                        if (infoAnteriorPerido is not null)
                                        {
                                            idUltimoPeriodo = infoAnteriorPerido.idPeriodoCuota;
                                        }
                                        else
                                        {
                                            posicionAnteriorPeriodo--;
                                            diferenciaPeriodos++;
                                        }

                                    } while (idUltimoPeriodo == 0);

                                    for (int dif = 0; dif <= diferenciaPeriodos; dif++)
                                    {
                                        idUltimoPeriodo = ObtenerPeriodoSegunPeriodo(infoMov.idTarjeta, idUltimoPeriodo, false);
                                    }
                                }
                                else
                                {
                                    var cantAnteriores = movCuotas.Where(x => x.idMovimiento.Equals(idMov) && x.nroCuota < i).ToList();
                                    var cantPosteriores = movCuotas.Where(x => x.idMovimiento.Equals(idMov) && x.nroCuota > i).ToList();

                                    bool usarAnterior = false;
                                    if (cantAnteriores.Count > 0)
                                    {
                                        var primerAnterior = cantAnteriores.Last();
                                        idUltimoPeriodo = primerAnterior.idPeriodoCuota;
                                        diferenciaPeriodos = i - primerAnterior.nroCuota;
                                    }
                                    else if (cantPosteriores.Count > 0)
                                    {
                                        var primerPosterior = cantPosteriores.First();
                                        idUltimoPeriodo = primerPosterior.idPeriodoCuota;
                                        diferenciaPeriodos = primerPosterior.nroCuota - 2;
                                        usarAnterior = true;
                                    }

                                    for (int dif = 1; dif <= diferenciaPeriodos; dif++)
                                    {
                                        idUltimoPeriodo = ObtenerPeriodoSegunPeriodo(infoMov.idTarjeta, idUltimoPeriodo, usarAnterior);
                                    }

                                }

                                var newMovCuotas = new tb_movimiento_cuota()
                                {
                                    idMovimiento = idMov,
                                    nroCuota = i,
                                    nroCuotas = infoMov.nroCuotas,
                                    nroCuotasPorPagar = infoMov.nroCuotas - i,
                                    TEA = tea,
                                    monto = infoMov.monto,
                                    montoCuota = montoCuota,
                                    montoPorPagar = 0,
                                    capital = 0,
                                    intereses = 0,
                                    idPeriodoCuota = idUltimoPeriodo
                                };

                                try
                                {
                                    var movCuota = cuentasDbContext.movimientoCuotas.Where(x => x.idMovimiento.Equals(idMov) &&
                                                                                           x.nroCuota.Equals(newMovCuotas.nroCuota) &&
                                                                                           x.nroCuotas.Equals(newMovCuotas.nroCuotas) &&
                                                                                           x.idPeriodoCuota.Equals(newMovCuotas.idPeriodoCuota)
                                                                ).FirstOrDefault();



                                    if (movCuota is null)
                                    {
                                        RegistrarMovimientoCuota(newMovCuotas);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public int ObtenerPeriodoSegunPeriodo(int? idTarjeta, int idPeriodo, bool anterior)
        {
            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            List<tb_tarjeta_periodo> periodos = new List<tb_tarjeta_periodo>();

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                periodos = cuentasDbContext.periodos.ToList();
            }

            var infoPeriodoActual = periodos.Where(x => x.id.Equals(idPeriodo)).FirstOrDefault();

            int idPeriodoObtenido = 0;

            if (infoPeriodoActual is not null)
            {
                int anio = infoPeriodoActual.anio;
                int mes = infoPeriodoActual.mes;

                if (anterior)
                {
                    if (mes == 1)
                    {
                        mes = 12;
                        anio--;
                    }
                    else
                        mes--;
                }
                else
                {
                    if (mes == 12)
                    {
                        mes = 1;
                        anio++;
                    }
                    else
                        mes++;
                }

                var infoPeriodoObtenido = periodos.Where(x => x.idTarjeta.Equals(idTarjeta) && x.anio.Equals(anio) && x.mes.Equals(mes)).FirstOrDefault();

                idPeriodoObtenido = infoPeriodoObtenido?.id ?? 0;
            }

            return idPeriodoObtenido;
        }

        public void GenerarReporte()
        {
            List<tb_movimiento> movimientos = new List<tb_movimiento>();
            List<tb_movimiento_cuota> movCuotas = new List<tb_movimiento_cuota>();
            List<tb_tarjeta_periodo> periodos = new List<tb_tarjeta_periodo>();
            List<tb_banco> bancos = new List<tb_banco>();
            List<tb_tarjeta> tarjetas = new List<tb_tarjeta>();

            var configBuilder = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").Build();

            var cuentasDbOptions = new DbContextOptionsBuilder<CuentasContext>()
                .UseSqlServer(configBuilder.GetConnectionString("DevConnection"))
                .Options;

            using (var cuentasDbContext = new CuentasContext(cuentasDbOptions))
            {
                periodos = cuentasDbContext.periodos.ToList();
                bancos = cuentasDbContext.bancos.ToList();
                movimientos = cuentasDbContext.movimientos.ToList();
                movCuotas = cuentasDbContext.movimientoCuotas.ToList();
                tarjetas = cuentasDbContext.tarjetas.ToList();
            }

            List<int> aniosReporte = new List<int> { 2022, 2023, 2024 };
            int idPersona = 1;
            List<Reporte> reporte = new List<Reporte>();

            foreach (int anio in aniosReporte)
            {
                for (int m = 1; m <= 12; m++)
                {
                    decimal totalCompras = 0;
                    decimal totalCuotas = 0;
                    decimal totalMesSoles = 0;
                    decimal totalMesDolares = 0;

                    var periodosMes = periodos.Where(x => x.anio.Equals(anio) && x.mes.Equals(m)).Select(y => y.id).ToList();
                    var movimientosPeriodo = movimientos.Where(x => periodosMes.Contains((int)x.idPeriodo)).ToList();
                    var movimientosCuotaPeriodo = movCuotas.Where(x => periodosMes.Contains(x.idPeriodoCuota)).Sum(x => x.montoCuota);

                    List<DetalleBanco> detalleBancos = new List<DetalleBanco>();

                    //foreach (var b in tarjetas)
                    //{
                    //    var infoBanco = bancos.Where(x => x.id.Equals(b.idBanco)).FirstOrDefault();

                    //    var totalMesTarjeta = movimientosPeriodo.Where(x => x.idTarjeta.Equals(b.id)).Sum(x => x.monto);

                    //    totalCompras += totalMesTarjeta;

                    //    detalleBancos.Add(new DetalleBanco()
                    //    {
                    //        idTarjeta = b.id,
                    //        descripcion = infoBanco?.descripcion?? "",
                    //        compras = totalMesTarjeta
                    //    });
                    //}

                    for (int idMoneda = 1; idMoneda <= 2; idMoneda++)
                    {
                        var movimientosMoneda = from mov in movimientos
                                                join p in periodos on mov.idPeriodo equals p.id
                                                where
                                                   mov.idMoneda.Equals(idMoneda) &&
                                                   mov.enCuotas.Equals(false) &&
                                                   mov.idBeneficiario.Equals(idPersona) &&
                                                   p.anio.Equals(anio) &&
                                                   p.mes.Equals(m)
                                                select mov.monto;

                        var cuotas = from mov in movimientos
                                     join movC in movCuotas on mov.id equals movC.idMovimiento
                                     join p in periodos on movC.idPeriodoCuota equals p.id
                                     where
                                        mov.idMoneda.Equals(idMoneda) &&
                                        mov.enCuotas.Equals(true) &&
                                        mov.idBeneficiario.Equals(idPersona) &&
                                        p.anio.Equals(anio) &&
                                        p.mes.Equals(m)
                                     select movC.montoCuota;

                        if (idMoneda == 1)
                        {
                            totalMesSoles += movimientosMoneda.Sum();
                            totalMesSoles += cuotas.Sum();
                        }
                        else
                        {
                            totalMesDolares += movimientosMoneda.Sum();
                            totalMesDolares += cuotas.Sum();
                        }

                    }

                    reporte.Add(new Reporte()
                    {
                        anio = anio,
                        mes = m,
                        descripcionMes = "",
                        totalCompras = totalCompras,
                        totalCuotas = totalCuotas,
                        totalMesSoles = totalMesSoles,
                        totalMesDolares = totalMesDolares,
                        detalleBancos = detalleBancos
                    });
                }

                //Console.WriteLine("Año " + anio);

                //foreach (var re in reporte)
                //{
                //    Console.WriteLine("Mes " + re.mes + " - " + re.totalMesSoles);
                //}

                //Console.WriteLine();

                //foreach (var re in reporte)
                //{
                //    Console.WriteLine("Mes " + re.mes + " - " + re.totalMesDolares);
                //}
            }

            for (int idMoneda = 1; idMoneda <= 2; idMoneda++)
            {
                for (int m = 1; m <= 12; m++)
                {
                    string linea = "Mes " + m;

                    foreach (int anio in aniosReporte)
                    {
                        linea += " ";
                        var infoRep = reporte.Where(x => x.mes.Equals(m) && x.anio.Equals(anio)).FirstOrDefault();

                        if (infoRep is not null)
                        {
                            linea += idMoneda == 1 ? infoRep.totalMesSoles : infoRep.totalMesDolares;
                        }
                        else
                        {
                            linea += 0;
                        }
                    }

                    Console.WriteLine(linea);
                }
            }
        }

        #region UTIL
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

        public DateTime ObtenerDiaInicioPeriodo(int anio, int mes, int diaInicio, int diaCorte, bool corteDiaHabil)
        {
            DateTime fechaInicio = new DateTime(anio, mes, diaCorte);

            if (mes == 1)
            {
                mes = 12;
                anio--;
            }
            else
                mes--;

            DateTime diaCorteAnterior = ObtenerDiaCortePeriodo(anio, mes, diaCorte, corteDiaHabil);

            fechaInicio = diaCorteAnterior.AddDays(1);

            return fechaInicio;
        }

        public DateTime ObtenerDiaCortePeriodo(int anio, int mes, int diaCorte, bool corteDiaHabil)
        {
            DateTime fechaCorte = new DateTime(anio, mes, diaCorte);

            if (corteDiaHabil)
            {
                if (fechaCorte.DayOfWeek == DayOfWeek.Saturday || fechaCorte.DayOfWeek == DayOfWeek.Sunday)
                {
                    bool esDiaHabil = false;

                    while (!esDiaHabil)
                    {
                        fechaCorte = fechaCorte.AddDays(-1);
                        esDiaHabil = !(fechaCorte.DayOfWeek == DayOfWeek.Saturday || fechaCorte.DayOfWeek == DayOfWeek.Sunday);
                    }
                }
            }

            return fechaCorte;
        }

        public DateTime ObtenerUltimoDiaPagoPeriodo(int anio, int mes, int diaCorte, bool corteDiaHabil)
        {
            DateTime fechaCorte = new DateTime(anio, mes, diaCorte);

            if (corteDiaHabil)
            {
                if (fechaCorte.DayOfWeek == DayOfWeek.Saturday || fechaCorte.DayOfWeek == DayOfWeek.Sunday)
                {
                    bool esDiaHabil = false;

                    while (!esDiaHabil)
                    {
                        fechaCorte = fechaCorte.AddDays(-1);
                        esDiaHabil = !(fechaCorte.DayOfWeek == DayOfWeek.Saturday || fechaCorte.DayOfWeek == DayOfWeek.Sunday);
                    }
                }
            }

            return fechaCorte;
        }

        private int ObtenerAnioFechaPorPeriodo(int anio, int mes)
        {
            int anioFecha = 0;

            return anioFecha;
        }
        #endregion
    }
}