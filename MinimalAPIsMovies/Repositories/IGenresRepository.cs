using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genres genre);
        Task<List<Genres>> GetAll();
        Task<Genres?> GetById(int id);
        Task<bool> Exists(int id);
        Task Update(Genres genre);
        Task Delete(int id);
        Task<List<int>> Exists(List<int> ids);
        Task<bool> Exists(int id, string name);
    }
}
