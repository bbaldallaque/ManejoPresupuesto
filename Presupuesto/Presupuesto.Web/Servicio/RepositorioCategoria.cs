using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Web.Models;
using System.Collections;

namespace Presupuesto.Web.Servicio
{
    public interface IRepositorioCategoria
    {
		Task Actualizar(Categoria categoria);
		Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtner(int usuarioId);
		Task<Categoria> ObtnerPorId(int id, int UsuarioId);
	}

    public class RepositorioCategoria : IRepositorioCategoria
    {
        private readonly string connectionString;

        public RepositorioCategoria(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }                                                              

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                                                           Values (@Nombre, @TipoOperacionId, @UsuarioId); 
                                                           SELECT SCOPE_IDENTITY();", categoria);
            categoria.Id = id;  
        }

        public async Task<IEnumerable<Categoria>> Obtner(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM Categorias WHERE UsuarioId = @UsuarioId", new { usuarioId });
        }

        public async Task<Categoria> ObtnerPorId(int id, int UsuarioId)
		{
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(
                @"SELECT * FROM Categorias WHERE Id = @Id AND UsuarioId = @UsuarioId", new { id, UsuarioId});
		}

        public async Task Actualizar(Categoria categoria)
		{
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                                          WHERE Id = @Id", categoria);
		}
    }
}
