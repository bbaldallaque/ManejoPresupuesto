using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Servicio
{
    public interface IRepositorioTransacciones
    {
        Task Crear(Transaccion transaccion);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
	{
		private readonly string connetionString;

		public RepositorioTransacciones(IConfiguration configuration)
		{
			connetionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task Crear(Transaccion transaccion)
		{
			using var connection = new SqlConnection(connetionString);
			var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar", 
				new { 
					transaccion.UsuarioId, 
					transaccion.FechaTransaccion,
					transaccion.Monto, 
					transaccion.CategoriaId, 
					transaccion.CuentaId,
					transaccion.Nota
				    }, 
				commandType: System.Data.CommandType.StoredProcedure);

			transaccion.Id = id;
		}
	}
}
