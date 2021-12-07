namespace Presupuesto.Web.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public int UsuarioId { get; set; }

        public int Orden { get; set; }
    }
}
