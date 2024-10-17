using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public class ActorsRepository : IActorsRepository
    {
        private readonly string connectionString;
        private readonly HttpContext httpcontext;
        public ActorsRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            httpcontext = httpContextAccessor.HttpContext!;
        }
        public async Task<int> Create(Actor Actors)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var id = await connection.QuerySingleAsync<int>("Actors_Create", new { Actors.Name,Actors.DateOfBirth,Actors.Picture });
                Actors.Id = id;
                return id;
            }
        }

        public async Task Delete(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Actors_Delete", new { Id }, commandType: System.Data.CommandType.StoredProcedure);

            }
        }

        public async Task<bool> Exists(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var Actors = await connection.QuerySingleAsync<bool>(@"Actors_Exist", new { Id }, commandType: System.Data.CommandType.StoredProcedure);
                return Actors;
            }
        }
        public async Task<List<Actor>> GetByName(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var Actors = await connection.QueryAsync<Actor>(@"Actors_GetByName", new { name }, commandType: System.Data.CommandType.StoredProcedure);
                return Actors.ToList();
            }
        }

        public async Task<List<Actor>> GetAll(PaginationDTO paginationDTO)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var Actors = await connection.QueryAsync<Actor>(@"Actors_GetAll",new { paginationDTO.Page,paginationDTO.RecoardsPerPage}, commandType: System.Data.CommandType.StoredProcedure);
                var Actors_count = await connection.QuerySingleAsync<int>(@"Actors_Count", commandType: System.Data.CommandType.StoredProcedure);
                httpcontext.Response.Headers.Append("totalAmountRecords", Actors_count.ToString());
                return Actors.ToList();
            }

        }

        public async Task<Actor?> GetById(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var Actors = await connection.QueryFirstOrDefaultAsync<Actor>(@"Actors_GetById", new { Id }, commandType: System.Data.CommandType.StoredProcedure);
                return Actors;
            }
        }

        public async Task Update(Actor Actors)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Actors_Update", new { Actors.Id, Actors.Name,Actors.DateOfBirth,Actors.Picture }, commandType: System.Data.CommandType.StoredProcedure);

            }
        }

    }
}
