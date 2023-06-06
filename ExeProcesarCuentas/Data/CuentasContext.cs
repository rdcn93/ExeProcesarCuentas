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
        public DbSet<tb_persona> personas { get; set; }
        public DbSet<tb_tarjeta> tarjetas { get; set; }
        public DbSet<tb_tarjeta_periodo> periodos { get; set; }
        public DbSet<tb_cuenta> cuentas { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(CuentasContext).Assembly);

        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.EnableSensitiveDataLogging();
        //}
    }
}
