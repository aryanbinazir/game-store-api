namespace GameStore.api.Dtos;

public record GameDto(
    int Id,
    string Name,
    int? GenreId,
    decimal Price,
    DateOnly? ReleaseDate
    );