namespace Presupuesto.Web.Models
{
	public class ResultadoObtnerPorMes
	{
		public int Mes { get; set; }

		public DateTime FechaReferencia { get; set; }

		public decimal Monto { get; set; }	

		public decimal Ingreso { get; set; }

		public decimal Gastos { get; set; }

		public TipoOperacion TipoOperacionId { get; set; }
	}
}
