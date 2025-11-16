using GameStore.api.Data;
using GameStore.api.Dtos.Genre;
using GameStore.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.Endpoints;

public static class GenresEndpoints
{
    private const string GetGenresByIdRouteName = "GetGenresById";
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres")
            .WithParameterValidation()
            .WithGroupName("Genres");
        
        // list all the genres
        group.MapGet("/", async (AppDbContext dbContext) =>
        {
            var genres = await dbContext.Genres
                .Include(genre => genre.Games)
                .Select(genre => new GenreDto(
                    genre.Id,
                    genre.Name,
                    genre.Games!.Select(game => game.Id).ToList()
                )).ToListAsync();
            return Results.Ok(genres);
        });

        // get a specific genre
        group.MapGet("/{id}", async (int id, AppDbContext dbContext) =>
        {
            var genre = await dbContext.Genres
                .Include(genre => genre.Games)
                .FirstOrDefaultAsync(genre => genre.Id == id);
            if (genre == null) return Results.NotFound();

            var result = new GenreDto(
                genre.Id,
                genre.Name,
                genre.Games?.Select(game=> game.Id).ToList());

            return Results.Ok(result);
        }).WithName(GetGenresByIdRouteName);

        // create a genre
        group.MapPost("/", async (CreateGenreDto dto, AppDbContext dbContext) =>
        {
            // Check about duplicate name
            if ( await dbContext.Genres.AnyAsync(genre => genre.Name == dto.Name))
                return Results.Conflict(new { message = "An item with that name already exists." });
            
            var games = new List<Game?>();
            if (dto.GamesId != null)
            {
                games = dto.GamesId
                    .Select(gameId =>  dbContext.Games.FirstOrDefault(game => game.Id == gameId))
                    .ToList();
                // Check for valid GamesId
                if (games.Any(game => game?.Id == null))
                    return Results.Conflict(new { message = "GamesId are not valid"});
            }
            
            Genre genre = new()
            {
                Name = dto.Name,
                Games = games!
            };
            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();

            var result = new GenreDto(genre.Id, genre.Name, genre.Games.Select(game => game.Id).ToList());
            return Results.CreatedAtRoute(GetGenresByIdRouteName, new {id = genre.Id}, result);
        });

        // update a genre
        group.MapPut("/{id}", async (int id, UpdateGenreDto dto, AppDbContext dbContext) =>
        {
            var genre = await dbContext.Genres
                .Include(genre => genre.Games)
                .FirstOrDefaultAsync(genre => genre.Id == id);
            if (genre == null)
            {
                return Results.NotFound();
            }
            
            if (dto.Name != null)
            {
                // check for duplicate names
                if (dbContext.Genres.Any(genre2 => genre2.Name == dto.Name))
                    return Results.Conflict(new { message = "An item with that name already exists." });
                genre.Name = dto.Name;
            }

            if (dto.GamesId != null)
            {
                var games = dto.GamesId
                    .Select(gameId => dbContext.Games.FirstOrDefault(game => game.Id == gameId))
                    .ToList();
                // Check for valid GamesId 
                if (games.Any(game => game?.Id == null))
                    return Results.Conflict(new { message = $"GamesId are not valid"});
                genre.Games = games!;
            }
            
            dbContext.Genres.Update(genre);
            await dbContext.SaveChangesAsync();
            
            var result = new GenreDto(genre.Id, genre.Name, genre.Games?.Select(game => game.Id).ToList());
            return Results.CreatedAtRoute(GetGenresByIdRouteName, new {id = genre.Id}, result);
        });

        // delete a genre
        group.MapDelete("/{id}", async (int id,  AppDbContext dbContext) =>
        {
            var genre = await dbContext.Genres
                .Include(genre => genre.Games)
                .FirstOrDefaultAsync(genre => genre.Id == id);
            if (genre is null) return Results.NotFound();
            dbContext.Genres.Remove(genre);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });
        return group;
    }
}