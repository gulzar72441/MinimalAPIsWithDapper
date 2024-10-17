using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly string connectionString;
        public GenresRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<int> Create(Genres genres)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var id = await connection.QuerySingleAsync<int>("Genres_Create", new { genres.Name});
                genres.Id = id;
                return id;
            }
        }

        public async Task Delete(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Genres_Delete", new { Id }, commandType: System.Data.CommandType.StoredProcedure);

            }
        }

        public async Task<bool> Exists(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QuerySingleAsync<bool>(@"Genres_Exist", new { Id }, commandType: System.Data.CommandType.StoredProcedure);
                return genres;
            }
        }

        public async Task<List<Genres>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryAsync<Genres>(@"Genres_GetAll", commandType:System.Data.CommandType.StoredProcedure);
                return genres.ToList(); 
            }

        }

        public async Task<Genres?> GetById(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryFirstOrDefaultAsync<Genres>(@"Genres_GetById", new { Id}, commandType: System.Data.CommandType.StoredProcedure);
                return genres;
            }
        }

        public async Task Update(Genres genres)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Genres_Update", new { genres.Id,genres.Name}, commandType: System.Data.CommandType.StoredProcedure);

            }
        }
    }
}
