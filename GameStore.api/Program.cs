using GameStore.api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


const string getGameByIdRouteName = "GetGameById";
List<GameDto> games =
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

// list all the game
app.MapGet("games/", () => games);

// get a specific game
app.MapGet("games/{id}", (int id) =>
{
    GameDto? game = games.Find(g => g.Id == id);
    return game is null ? Results.NotFound() : Results.Ok(game);
}).WithName(getGameByIdRouteName);

// create a game
app.MapPost("games/create", (CreateGameDto dto) =>
{
    
    var game = new GameDto(
        games[^1].Id + 1,
        dto.Name,
        dto.Genre,
        dto.Price,
        dto.ReleaseDate
    );
    games.Add(game);
    
    return Results.CreatedAtRoute(getGameByIdRouteName, new {id = game.Id}, game);
});

// update a game
app.MapPut("games/update/{id}", (int id, UpdateGameDto dto) =>
{
    var index = games.IndexOf(games.Find(g => g.Id == id)!);
    if (index == -1)
    {
        return Results.NotFound();
    }
    
    games[index] = new GameDto(id, dto.Name, dto.Genre, dto.Price, dto.ReleaseDate);
    return Results.CreatedAtRoute(getGameByIdRouteName, new {id = games[index].Id}, games[index]);
    
});

// delete a game
app.MapDelete("games/delete/{id}", (int id) =>
{
    var game = games.Find(g => g.Id == id);
    if (game is null) return Results.NotFound();
    games.Remove(game);
    return Results.NoContent();
});
    
app.Run();