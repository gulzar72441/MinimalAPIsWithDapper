using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor genres);
        Task<List<Actor>> GetAll(PaginationDTO paginationDTO);
        Task<Actor?> GetById(int Id);
        Task<bool> Exists(int Id);
        Task Update(Actor genres);
        Task Delete(int Id);
        Task<List<Actor>> GetByName(string name);
    }
}
