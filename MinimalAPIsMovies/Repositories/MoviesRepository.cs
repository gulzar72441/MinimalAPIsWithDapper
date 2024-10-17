using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly string connectionString;
        private readonly HttpContext httpcontext;
        public MoviesRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            httpcontext = httpContextAccessor.HttpContext!;
        }
        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var id = await connection.QuerySingleAsync<int>("Movies_Create", new { movie.Title, movie.InTheaters,movie.ReleaseDate, movie.Poster }, commandType:System.Data.CommandType.StoredProcedure);
                movie.Id = id;
                return id;
            }
        }
        public async Task Delete(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Movies_Delete", new { Id }, commandType: System.Data.CommandType.StoredProcedure);

            }
        }

        public async Task<bool> Exists(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movies = await connection.QuerySingleAsync<bool>(@"Movies_Exist", new { Id }, commandType: System.Data.CommandType.StoredProcedure);
                return movies;
            }
        }
        public async Task<List<Movie>> GetByName(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movies = await connection.QueryAsync<Movie>(@"Movies_GetByName", new { name }, commandType: System.Data.CommandType.StoredProcedure);
                return movies.ToList();
            }
        }

        public async Task<List<Movie>> GetAll(PaginationDTO paginationDTO)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movies = await connection.QueryAsync<Movie>(@"Movies_GetAll", new { paginationDTO.Page, paginationDTO.RecoardsPerPage }, commandType: System.Data.CommandType.StoredProcedure);
                var movies_count = await connection.QuerySingleAsync<int>(@"Movies_Count", commandType: System.Data.CommandType.StoredProcedure);
                httpcontext.Response.Headers.Append("totalAmountRecords", movies_count.ToString());
                return movies.ToList();
            }

        }

        public async Task<Movie?> GetById(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movie = await connection.QueryFirstOrDefaultAsync<Movie>(@"Movies_GetById", new { Id }, commandType: System.Data.CommandType.StoredProcedure);
                return movie;
            }
        }

        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Movies_Update", new { movie.Id, movie.Title, movie.InTheaters, movie.ReleaseDate,movie.Poster }, commandType: System.Data.CommandType.StoredProcedure);

            }
        }
    }
}
