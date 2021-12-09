namespace Presupuesto.Web.Servicio
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        public int ObtenerUsuarioId()
        {
           return 1;
        }
    }
}
