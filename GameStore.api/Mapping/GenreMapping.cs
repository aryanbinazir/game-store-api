using GameStore.api.Dtos.Genre;
using GameStore.api.Entities;

namespace GameStore.api.Mapping;

public static class GenreMapping
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto(
            genre.Id,
            genre.Name,
            genre.Games!.Select(game => game.Id).ToList()
            );
    }
}