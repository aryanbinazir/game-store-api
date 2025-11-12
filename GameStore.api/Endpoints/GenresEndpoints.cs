using GameStore.api.Data;
using GameStore.api.Dtos.Genre;
using GameStore.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.Endpoints;

public static class GenresEndpoints
{
    
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres")
                       .WithParameterValidation();
        
        // list all the genres
        group.MapGet("/", (AppDbContext dbContext) =>
        {
            var genres = dbContext.Genres
                .Include(genre => genre.Games)
                .Select(genre => new GenreDto(
                    genre.Id,
                    genre.Name,
                    genre.Games!.Select(game => game.Name).ToList()
                ));
            return Results.Ok(genres);
        });

        // get a specific genre
        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
        {
            var genre = dbContext.Genres
                .Include(genre => genre.Games)
                .FirstOrDefault(genre => genre.Id == id);
            if (genre == null) return Results.NotFound();

            var result = new GenreDto(
                genre.Id,
                genre.Name,
                genre.Games!.Select(game=> game.Name).ToList());

            return Results.Ok(result);
        });

        // create a genre
        group.MapPost("/", (CreateGenreDto dto, AppDbContext dbContext) =>
        {
            // Check about duplicate name
            if (dbContext.Genres.Any(genre => genre.Name == dto.Name))
                return Results.Conflict(new { message = "An item with that name already exists." });
            
            var games = new List<Game?>();
            if (dto.GamesId != null)
            {
                games = dto.GamesId
                    .Select(gameId => dbContext.Games.FirstOrDefault(game => game.Id == gameId))
                    .ToList();
            }
            // Check for valid GamesId
            if (games.Any(game => game?.Id == null))
                return Results.Conflict(new { message = $"GamesId are not valid"});
            

            Genre genre = new()
            {
                Name = dto.Name,
                Games = games!
            };
            dbContext.Genres.Add(genre);
            dbContext.SaveChanges();

            var result = new GenreDto(genre.Id, genre.Name, genre.Games.Select(game => game.Name).ToList());
            return Results.Ok(result);
        });

        // update a genre
        group.MapPut("/{id}", (int id, UpdateGenreDto dto, AppDbContext dbContext) =>
        {
            var genre = dbContext.Genres
                .Include(genre => genre.Games)
                .FirstOrDefault(genre => genre.Id == id);
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
            dbContext.SaveChanges();
            
            var result = new GenreDto(genre.Id, genre.Name, genre.Games?.Select(game => game.Name).ToList());
            return Results.Ok(result);
        });

        // delete a genre
        group.MapDelete("/{id}", (int id,  AppDbContext dbContext) =>
        {
            var genre = dbContext.Genres.FirstOrDefault(genre => genre.Id == id);
            if (genre is null) return Results.NotFound();
            dbContext.Genres.Remove(genre);
            dbContext.SaveChanges();
            return Results.Ok();
        });
        return group;
    }
}