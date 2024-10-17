using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genres genres);
        Task<List<Genres>> GetAll();
        Task<Genres?> GetById(int Id);
        Task<bool> Exists(int Id);
        Task Update(Genres genres);
        Task Delete(int Id);
    }
}
