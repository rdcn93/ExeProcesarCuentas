namespace ExeProcesarCuentas.Classes
{
    internal class Reporte
    {
        public string descripcionMes { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public decimal totalCompras { get; set; }
        public decimal totalCuotas { get; set; }
        public decimal totalMesSoles { get; set; }
        public decimal totalMesDolares { get; set; }
        public List<DetalleBanco> detalleBancos { get; set; }
    }

    internal class DetalleBanco
    {
        public int idTarjeta { get; set; }
        public string descripcion { get; set; }
        public decimal compras { get; set; }
        public decimal cuotas { get; set; }
    }
}
