using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.EndPoints
{
    public static class MoviesEndPoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetMovies).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/{name}", GetByName);
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }
        #region Mehtods
        static async Task<Ok<List<MovieDTO>>> GetMovies(IMoviesRepository moviesRepository, IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecoardsPerPage = recordsPerPage };
            var movies = await moviesRepository.GetAll(pagination);
            var movieDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(movieDTO);
        }
        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository moviesRepository, IMapper mapper)
        {
            var movie = await moviesRepository.GetById(id);
            if (movie is null)
                return TypedResults.NotFound();
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
        }
        static async Task<Results<Ok<List<MovieDTO>>, NotFound>> GetByName(string name, IMoviesRepository moviesRepository, IMapper mapper)
        {
            var movie = await moviesRepository.GetByName(name);
            var movieDTO = mapper.Map<List<MovieDTO>>(movie);
            return TypedResults.Ok(movieDTO);
        }
        static async Task<Created<Movie>> Create([FromForm] CreateMovieDTO createMovieDTO, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
        {
            var movie = mapper.Map<Movie>(createMovieDTO);
            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Store(container, createMovieDTO.Poster);
                movie.Poster = url;
            }
            await moviesRepository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.Created($"/movies/{movie.Id}", movie);
        }
        static async Task<Results<NotFound, NoContent>> Update(int id, [FromForm] CreateMovieDTO createMovieDTO, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
        {
            var exists = await moviesRepository.GetById(id);
            if (exists is null)
                return TypedResults.NotFound();
            var movie = mapper.Map<Movie>(createMovieDTO);
            movie.Id = id;
            movie.Poster = exists.Poster;
            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movie.Poster, container, createMovieDTO.Poster);
                movie.Poster = url;
            }
            await moviesRepository.Update(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NotFound, NoContent>> Delete(int id, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
        {
            var exists = await moviesRepository.GetById(id);
            if (exists is null)
                return TypedResults.NotFound();
            await moviesRepository.Delete(id);
            await fileStorage.Delete(exists.Poster, container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }
        #endregion
    }
}
