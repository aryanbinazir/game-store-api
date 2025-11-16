using GameStore.api.Entities;

namespace GameStore.api.Dtos.Genre;

public record GenreDto(int Id, string Name, List<int>? GamesId)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public List<int>? GamesId { get; set; } = GamesId;
}