using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Servicio
{
    public interface IRepositorioCuenta
    {
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
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
            return await connetion.QueryAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Cuentas.Balance, tc.Nombre as TipoCuenta
                                                        from Cuentas Inner Join TipoCuentas tc
                                                        on tc.Id = Cuentas.TipoCuentaId
                                                        Where tc.UsuarioId = @usuarioId
                                                        Order by tc.Orden", new { usuarioId });
        }
    }
}
