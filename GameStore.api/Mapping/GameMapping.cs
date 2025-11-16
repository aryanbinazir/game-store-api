using GameStore.api.Data;
using GameStore.api.Dtos.Game;
using GameStore.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.Mapping;

public static class GameMapping
{
    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Name,
            game.Genre?.Id,
            game.Price,
            game.ReleaseDate);
    }
}