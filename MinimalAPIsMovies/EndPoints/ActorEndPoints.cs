using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.EndPoints
{
    public static class ActorEndPoints
    {
        private readonly static string container = "actors";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetActors).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("actor-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/{name}", GetByName);
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }
        #region Mehtods
        static async Task<Ok<List<ActorDTO>>> GetActors(IActorsRepository actorsRepository, IMapper mapper,int page =1 , int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecoardsPerPage = recordsPerPage };
            var actors = await actorsRepository.GetAll(pagination);
            var actorDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorDTO);
        }
        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById(int id, IActorsRepository actorsRepository, IMapper mapper)
        {
            var actor = await actorsRepository.GetById(id);
            if (actor is null)
                return TypedResults.NotFound();
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }
        static async Task<Results<Ok<List<ActorDTO>>, NotFound>> GetByName(string name, IActorsRepository actorsRepository, IMapper mapper)
        {
            var actor = await actorsRepository.GetByName(name);
            var actorDTO = mapper.Map<List<ActorDTO>>(actor);
            return TypedResults.Ok(actorDTO);
        }
        static async Task<Created<Actor>> Create([FromForm] CreateActorDTO createActorDTO, IActorsRepository actorsRepository, IOutputCacheStore outputCacheStore, IMapper mapper,IFileStorage fileStorage)
        {
            var actors = mapper.Map<Actor>(createActorDTO);
            if (createActorDTO.Picture is not null)
            {
                var url = await fileStorage.Store(container, createActorDTO.Picture);
                actors.Picture = url;
            }
            await actorsRepository.Create(actors);
            await outputCacheStore.EvictByTagAsync("actor-get", default);
            return TypedResults.Created($"/actors/{actors.Id}", actors);
        }
        static async Task<Results<NotFound, NoContent>> Update(int id, [FromForm] CreateActorDTO createActorDTO, IActorsRepository actorsRepository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
        {
            var exists = await actorsRepository.GetById(id);
            if (exists is null)
                return TypedResults.NotFound();
            var actor = mapper.Map<Actor>(createActorDTO);
            actor.Id = id;
            actor.Picture = exists.Picture;
            if (createActorDTO.Picture is not null)
            {
                var url = await fileStorage.Edit(actor.Picture, container, createActorDTO.Picture);
                actor.Picture = url;
            }
            await actorsRepository.Update(actor);
            await outputCacheStore.EvictByTagAsync("actor-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NotFound, NoContent>> Delete(int id, IActorsRepository actorsRepository, IOutputCacheStore outputCacheStore,IFileStorage fileStorage)
        {
            var exists = await actorsRepository.GetById(id);
            if (exists is null)
                return TypedResults.NotFound();
            await actorsRepository.Delete(id);
            await fileStorage.Delete(exists.Picture, container);
            await outputCacheStore.EvictByTagAsync("actor-get", default);
            return TypedResults.NoContent();
        }
        #endregion
    }
}
