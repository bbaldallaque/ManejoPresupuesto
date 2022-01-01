using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Servicio
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId);
		Task Borrar(int id);
		Task Crear(Transaccion transaccion);
		Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
		Task<Transaccion> ObtenerPorId(int id, int usuarioId);
		Task<IEnumerable<ResultadoObtnerPorMes>> ObtenerPorMes(int usuarioId, int año);
		Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParamentroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParamentroObtenerTransaccionesPorUsuario modelo);
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
                new
                {
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

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connetionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",

                  new
                  {
                      transaccion.Id,
                      transaccion.FechaTransaccion,
                      transaccion.Monto,
                      transaccion.CategoriaId,
                      transaccion.CuentaId,
                      transaccion.Nota,
                      montoAnterior,
                      cuentaAnteriorId
                  }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connetionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT Transacciones. *, cat.TipoOperacionId 
                FROM Transacciones
                INNER JOIN Categorias cat
                ON cat.Id = Transacciones.CategoriaId
                WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId", new { id, usuarioId });
        }

        public async Task Borrar(int id)
		{
            using var connection = new SqlConnection(connetionString);
            await connection.ExecuteAsync("Transacciones_Borrar",
            new {id}, commandType: System.Data.CommandType.StoredProcedure);
		}

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
		{
            using var connection = new SqlConnection(connetionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, T.Monto, t.FechaTransaccion, c.Nombre as Categoria,
                                                            cu.Nombre as Cuenta, c.TipoOperacionId
                                                            FROM Transacciones t
                                                            INNER JOIN Categorias c
                                                            ON c.Id = t.CategoriaId
                                                            INNER JOIN Cuentas cu
                                                            ON cu.Id = t.CuentaId
                                                            WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                                                            AND  FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
		}

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParamentroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connetionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, T.Monto, t.FechaTransaccion, c.Nombre as Categoria,
                                                            cu.Nombre as Cuenta, c.TipoOperacionId
                                                            FROM Transacciones t
                                                            INNER JOIN Categorias c
                                                            ON c.Id = t.CategoriaId
                                                            INNER JOIN Cuentas cu
                                                            ON cu.Id = t.CuentaId
                                                            WHERE t.UsuarioId = @UsuarioId
                                                            AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                                                            ORDER BY t.FechaTransaccion DESC", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParamentroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connetionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"Select DATEDIFF(d, @fechaInicio, @fechafin) / 7 + 1 as Semana,
                                                                            sum(Monto) as Monto, cat.TipoOperacionId
                                                                            from Transacciones
                                                                            Inner Join Categorias cat
                                                                            On cat.Id = Transacciones.CategoriaId
                                                                            Where  transacciones.UsuarioId = @usuarioId AND
                                                                            FechaTransaccion Between @fechaInicio AND @fechaFin
                                                                            Group by DATEDIFF(d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId", modelo);
        }

        public async Task<IEnumerable<ResultadoObtnerPorMes>> ObtenerPorMes(int usuarioId, int año)
		{
            using var connection = new SqlConnection(connetionString);
            return await connection.QueryAsync<ResultadoObtnerPorMes>(@"SELECT MONTH(FechaTransaccion) as Mes,
                                                                    SUM(Monto) as Monto, cat.TipoOperacionId
                                                                    FROM Transacciones
                                                                    INNER JOIN Categorias cat
                                                                    ON cat.Id = Transacciones.CategoriaId
                                                                    WHERE Transacciones.UsuarioId = @usuarioId And YEAR(FechaTransaccion) = @Año
                                                                    GROUP BY MONTH(fechaTransaccion), cat.TipoOperacionId", new { usuarioId, año });
		}

    }
}

