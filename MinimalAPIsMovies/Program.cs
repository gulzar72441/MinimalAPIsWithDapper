using MinimalAPIsMovies.EndPoints;
using MinimalAPIsMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddTransient<IMoviesRepository, MoviesRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.UseStaticFiles();
app.UseOutputCache();
app.UseCors();
app.Run();
