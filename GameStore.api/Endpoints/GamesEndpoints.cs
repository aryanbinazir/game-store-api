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
        group.MapGet("/", async (AppDbContext dbContext) =>
        {
            var games = await dbContext.Games
                .Include(game => game.Genre)
                .Select(game => new GameDto(
                    game.Id,
                    game.Name,
                    game.Genre!.Id,
                    game.Price,
                    game.ReleaseDate
                )).ToListAsync();
            return Results.Ok(games);
        });

        // get a specific game
        group.MapGet("/{id}", async (int id, AppDbContext dbContext) =>
        {
            var game = await dbContext.Games
                .Include(game => game.Genre)
                .FirstOrDefaultAsync(game => game.Id == id);
            if (game == null) return Results.NotFound();

            var result = new GameDto(
                game.Id,
                game.Name,
                game.Genre?.Id,
                game.Price,
                game.ReleaseDate
            );

            return Results.Ok(result);
        }).WithName(GetGamesByIdRouteName);

        // create a game
        group.MapPost("/", async (CreateGameDto dto, AppDbContext dbContext) =>
        {
            // Check about duplicate name and invalid IdGenre
            if (await dbContext.Games.AnyAsync(game => game.Name == dto.Name))
                return Results.Conflict(new { message = "An item with that name already exists." });

            if (dto.IdGenre != null)
            {
                var genre = await dbContext.Genres.FirstOrDefaultAsync(genre => genre.Id == dto.IdGenre);
                if (genre == null)
                    return Results.Conflict(new { message = "IdGenre is not valid" });
            }
            
            Game game = new()
            {
                Name = dto.Name,
                GenreId = dto.IdGenre,
                Genre = await dbContext.Genres.FirstOrDefaultAsync(genre => genre.Id == dto.IdGenre),
                Price = dto.Price,
                ReleaseDate = dto.ReleaseDate
            };
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            var result = new GameDto(game.Id, game.Name, game.Genre?.Id, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGamesByIdRouteName, new {id = game.Id}, result);
        });

        // update a game
        group.MapPut("/{id}", async (int id, UpdateGameDto dto, AppDbContext dbContext) =>
        {
            var game = await dbContext.Games
                .Include(game => game.Genre)
                .FirstOrDefaultAsync(game => game.Id == id);
            if (game == null)
            {
                return Results.NotFound();
            }
    
            if (dto.Name != null)
            {
                // Check input name is duplicate or not
                if (await dbContext.Games.AnyAsync(game2 => game2.Name == dto.Name))
                    return Results.Conflict(new { message = "An item with that name already exists." });
                game.Name = dto.Name;
            }
            if (dto.Price != null) game.Price = (decimal)dto.Price;
            if (dto.IdGenre != null)
            {
                // Check is that Genre with the id has given is valid or not
                var genre = await dbContext.Genres.FirstOrDefaultAsync(genre => genre.Id == dto.IdGenre);
                if (genre == null) 
                    return Results.Conflict(new { message = "IdGenre is not valid" });
                game.GenreId = dto.IdGenre;
                game.Genre = genre;
            }
            if (dto.ReleaseDate != null) game.ReleaseDate = dto.ReleaseDate;
            
            dbContext.Games.Update(game);
            await dbContext.SaveChangesAsync();
            
            var result = new GameDto(game.Id, game.Name, game.Genre?.Id, game.Price, game.ReleaseDate);
            return Results.CreatedAtRoute(GetGamesByIdRouteName, new {id = game.Id}, result);
        });

        // delete a game
        group.MapDelete("/{id}", async (int id,  AppDbContext dbContext) =>
        {
            var game = await dbContext.Games.FirstOrDefaultAsync(genre => genre.Id == id);
            if (game is null) return Results.NotFound();
            dbContext.Games.Remove(game);
            await dbContext.SaveChangesAsync();
            return Results.Accepted();
        });
        return group;
        
        
    }
}