namespace Presupuesto.Web.Models
{
	public class ReporteMensualViewModel
	{
		public IEnumerable<ResultadoObtnerPorMes> TransaccionesPorMes { get; set; }

		public decimal Ingesos => TransaccionesPorMes.Sum(x => x.Ingreso);

		public decimal Gastos => TransaccionesPorMes.Sum(x => x.Gastos);

		public decimal Total => Ingesos - Gastos;

		public int Año { get; set; }
	}
}
