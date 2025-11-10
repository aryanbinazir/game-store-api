using GameStore.api.Entities;

namespace GameStore.api.Dtos.Genre;

public record CreateGenreDto
{
    public required string Name { get; set; }
    public List<int>? GamesId { get; set; }
}