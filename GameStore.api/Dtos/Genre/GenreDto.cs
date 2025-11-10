using GameStore.api.Entities;

namespace GameStore.api.Dtos.Genre;

public record GenreDto(int Id, string Name, List<string>? GamesName)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public List<string>? GamesName { get; set; } = GamesName;
}