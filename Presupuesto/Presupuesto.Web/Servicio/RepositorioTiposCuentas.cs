using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Servicio
{
    public interface IRepositorioTiposCuentas
    {
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task  Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TipoCuentas (Nombre, UsuarioId, Orden)
                                                 Values (@Nombre, @UsuarioId, 0);
                                                 SELECT SCOPE_IDENTITY();", tipoCuenta);
              tipoCuenta.Id = id;   
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connectiong = new SqlConnection(connectionString);
            var existe = await connectiong.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM TipoCuentas
                                           WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;",
                                           new {nombre, usuarioId});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, orden 
                                    FROM TipoCuentas 
                                    WHERE UsuarioId = @UsuarioId", new {usuarioId});
        }
    }
}
