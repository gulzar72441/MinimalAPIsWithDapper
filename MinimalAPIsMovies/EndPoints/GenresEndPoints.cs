using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.EndPoints
{
    public static class GenresEndPoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("genre-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id:int}", Delete);
            return group;
        }
        #region Mehtods
        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository genresRepository,IMapper mapper)
        {
            var genres = await genresRepository.GetAll();
            var genreDTO = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genreDTO);
        }
        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository genresRepository, IMapper mapper)
        {
            var genre = await genresRepository.GetById(id);
            if (genre is null)
                return TypedResults.NotFound();
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDTO);
        }
        static async Task<Created<Genres>> Create(CreateGenreDTO createGenreDTO, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genres = mapper.Map<Genres>(createGenreDTO);
            await genresRepository.Create(genres);
            await outputCacheStore.EvictByTagAsync("genre-get", default);
            return TypedResults.Created($"/genres/{genres.Id}", genres);
        }
        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenreDTO, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var exists = await genresRepository.GetById(id);
            if (exists is null)
                return TypedResults.NotFound();
            var genres = mapper.Map<Genres>(createGenreDTO);
            genres.Id = id;
            await genresRepository.Update(genres);
            await outputCacheStore.EvictByTagAsync("genre-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore)
        {
            var exists = await genresRepository.GetById(id);
            if (exists is null)
                return TypedResults.NotFound();
            await genresRepository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genre-get", default);
            return TypedResults.NoContent();
        }
        #endregion
    }

}
