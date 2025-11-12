using GameStore.api.Data;
using GameStore.api.Dtos;
using GameStore.api.Entities;
using Microsoft.EntityFrameworkCore;


namespace GameStore.api.Endpoints;

public static class GamesEndpoints
{
    private const string GetGamesByIdRouteName = "GetGamesById";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
            .WithParameterValidation()
            .WithGroupName("Games");
        
        // list all the game
        group.MapGet("/", (AppDbContext dbContext) =>
        {
            var games = dbContext.Games
                .Include(game => game.Genre)
                .Select(game => new GameDto(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                ));
            return Results.Ok(games);
        });

        // get a specific game
        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
        {
            var game = dbContext.Games
                .Include(game => game.Genre)
                .FirstOrDefault(game => game.Id == id);

            return game == null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGamesByIdRouteName);

        // create a game
        group.MapPost("/", (CreateGameDto dto, AppDbContext dbContext) =>
        {
            // Check about duplicate name and invalid IdGenre
            if (dbContext.Games.Any(game => game.Name == dto.Name))
                return Results.Conflict(new { message = "An item with that name already exists." });
            
            var genre = dbContext.Genres.FirstOrDefault(genre => genre.Id == dto.IdGenre);
            if (genre == null) 
                return Results.Conflict(new { message = "IdGenre is not valid" });
    
            Game game = new()
            {
                Name = dto.Name,
                GenreId = dto.IdGenre,
                Genre = dbContext.Genres.FirstOrDefault(genre2 => genre2.Id == dto.IdGenre),
                Price = dto.Price,
                ReleaseDate = dto.ReleaseDate
            };
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            var result = new GameDto(game.Id, game.Name, game.Genre?.Name, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGamesByIdRouteName, new {id = game.Id}, result);
        });

        // update a game
        group.MapPut("/{id}", (int id, UpdateGameDto dto, AppDbContext dbContext) =>
        {
            var game =  dbContext.Games
                .Include(game => game.Genre)
                .FirstOrDefault(game => game.Id == id);
            if (game == null)
            {
                return Results.NotFound();
            }
    
            if (dto.Name != null)
            {
                // Check input name is duplicate or not
                if (dbContext.Games.Any(game2 => game2.Name == dto.Name))
                    return Results.Conflict(new { message = "An item with that name already exists." });
                game.Name = dto.Name;
            }
            if (dto.Price != null) game.Price = (decimal)dto.Price;
            if (dto.IdGenre != null)
            {
                // Check is that Genre with the id has given is valid or not
                var genre = dbContext.Genres.FirstOrDefault(genre => genre.Id == dto.IdGenre);
                if (genre == null) 
                    return Results.Conflict(new { message = "IdGenre is not valid" });
                game.GenreId = dto.IdGenre;
                game.Genre = genre;
            }
            if (dto.ReleaseDate != null) game.ReleaseDate = dto.ReleaseDate;
            
            dbContext.Games.Update(game);
            dbContext.SaveChanges();
            
            var result = new GameDto(game.Id, game.Name, game.Genre?.Name, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGamesByIdRouteName, new {id = game.Id}, result);
        });

        // delete a game
        group.MapDelete("/{id}", (int id,  AppDbContext dbContext) =>
        {
            var game = dbContext.Games.FirstOrDefault(genre => genre.Id == id);
            if (game is null) return Results.NotFound();
            dbContext.Games.Remove(game);
            dbContext.SaveChanges();
            return Results.Accepted();
        });

        group.MapDelete("/", (AppDbContext dbContext) =>
        {
            var games = dbContext.Games.ToList();
            foreach (var game in games)
            {
                dbContext.Games.Remove(game);
            }
            dbContext.SaveChanges();
            return Results.Accepted();
        });
        return group;
        
        
    }
}