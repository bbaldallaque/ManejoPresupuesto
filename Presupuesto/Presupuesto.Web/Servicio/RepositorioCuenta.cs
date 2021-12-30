using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Servicio
{
    public interface IRepositorioCuenta
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuenta : IRepositorioCuenta 
    {
        private readonly string connetionString;

        public RepositorioCuenta(IConfiguration configuration)
        {
            connetionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connction = new SqlConnection(connetionString);
            var id = await connction.QuerySingleAsync<int>(@"INSERT INTO Cuentas(Nombre, TipoCuentaId, Descripcion, Balance)
                                                    VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                                                    SELECT SCOPE_IDENTITY()", cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connetion = new SqlConnection(connetionString);
            return await connetion.QueryAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                                                        FROM Cuentas Inner Join TipoCuentas tc
                                                        on tc.Id = Cuentas.TipoCuentaId
                                                        Where tc.UsuarioId = @usuarioId
                                                        Order by tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connetionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, TipoCuentaId
                                                        FROM Cuentas 
                                                        Inner Join TipoCuentas tc
                                                        ON tc.Id = Cuentas.TipoCuentaId
                                                        WHERE tc.UsuarioId = @usuarioId AND Cuentas.Id = @Id", new {id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connetionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                                          SET Nombre =  @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                                          WHERE Id = @Id;", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connetionString);
            await connection.ExecuteAsync("DELETE Cuentas WHERE Id = @Id", new { id });
        }
    }
}
