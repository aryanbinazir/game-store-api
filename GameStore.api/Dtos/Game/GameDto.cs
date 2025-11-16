namespace GameStore.api.Dtos.Game;

public record GameDto(
    int Id,
    string Name,
    int? GenreId,
    decimal Price,
    DateOnly? ReleaseDate
    );