using GameStore.api.Dtos;

namespace GameStore.api.Endpoints;

public static class GamesEndpoints
{
    private const string GetGameByIdRouteName = "GetGameById";

    private static readonly List<GameDto> Games =
    [
        new GameDto(
            1,
            "Elden Ring",
            "RPG",
            59.99m,
            new DateOnly(2022, 2, 25)),
        new GameDto(
            2,
            "FIFA 24",
            "Sports",
            69.99m,
            new DateOnly(2023, 9, 29)),
        new GameDto(
            3,
            "Call of Duty: Modern Warfare III",
            "Shooter",
            79.99m,
            new DateOnly(2023, 11, 10))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games");
        
        // list all the game
        group.MapGet("/", () => Games);

        // get a specific game
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = Games.Find(g => g.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameByIdRouteName);

        // create a game
        group.MapPost("/", (CreateGameDto dto) =>
        {
    
            var game = new GameDto(
                Games[^1].Id + 1,
                dto.Name,
                dto.Genre,
                dto.Price,
                dto.ReleaseDate
            );
            Games.Add(game);
    
            return Results.CreatedAtRoute(GetGameByIdRouteName, new {id = game.Id}, game);
        });

        // update a game
        group.MapPut("/{id}", (int id, UpdateGameDto dto) =>
        {
            var index = Games.IndexOf(Games.Find(g => g.Id == id)!);
            if (index == -1)
            {
                return Results.NotFound();
            }
    
            Games[index] = new GameDto(id, dto.Name, dto.Genre, dto.Price, dto.ReleaseDate);
            return Results.CreatedAtRoute(GetGameByIdRouteName, new {id = Games[index].Id}, Games[index]);
    
        });

        // delete a game
        group.MapDelete("/{id}", (int id) =>
        {
            var game = Games.Find(g => g.Id == id);
            if (game is null) return Results.NotFound();
            Games.Remove(game);
            return Results.NoContent();
        });
        return group;
    }
}