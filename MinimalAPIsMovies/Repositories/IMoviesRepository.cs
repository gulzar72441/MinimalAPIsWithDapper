using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IMoviesRepository
    {
        Task<int> Create(Movie movie);
        Task Delete(int Id);
        Task<bool> Exists(int Id);
        Task<List<Movie>> GetAll(PaginationDTO paginationDTO);
        Task<Movie?> GetById(int Id);
        Task<List<Movie>> GetByName(string name);
        Task Update(Movie movie);
    }
}
