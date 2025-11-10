using GameStore.api.Entities;

namespace GameStore.api.Dtos.Genre;

public record UpdateGenreDto
{
    public string? Name { get; set; }
    public List<int>? GamesId { get; set; }
}