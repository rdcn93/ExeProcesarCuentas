using ExeProcesarCuentas.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeProcesarCuentas.Data
{
    
    class CuentasContext : DbContext
    {
        public CuentasContext(DbContextOptions<CuentasContext> options) : base(options)
        {
        }

        public DbSet<tb_moneda> monedas { get; set; }
        public DbSet<tb_pais> paises { get; set; }
        public DbSet<tb_categoria> categorias { get; set; }
        public DbSet<tb_movimiento> movimientos { get; set; }
        public DbSet<tb_movimiento_cuota> movimientoCuotas { get; set; }
        public DbSet<tb_persona> personas { get; set; }
        public DbSet<tb_tarjeta> tarjetas { get; set; }
        public DbSet<tb_tarjeta_periodo> periodos { get; set; }
        public DbSet<tb_cuenta> cuentas { get; set; }
        public DbSet<tb_banco> bancos { get; set; }
        public DbSet<tb_periodo_configuracion> periodosConfiguracion { get; set; }
        public DbSet<tb_prestamo> prestamos { get; set; }

        public virtual DbSet<tb_tarjeta_periodo_pago> periodoPagos { get; set; }
        public virtual DbSet<tb_prestamo_detalle> prestamoDetalles { get; set; }
        public virtual DbSet<tb_prestamo_pago> prestamoPagos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tb_prestamo_detalle>().HasKey(x => new { x.idMovimiento, x.idPrestamo });
            modelBuilder.Entity<tb_prestamo_pago>().HasKey(x => new { x.idMovimiento, x.idPrestamo });
            modelBuilder.Entity<tb_tarjeta_periodo_pago>().HasKey(x => new { x.idMovimiento, x.idPeriodo });

            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(CuentasContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
