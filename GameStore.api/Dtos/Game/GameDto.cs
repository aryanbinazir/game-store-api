namespace GameStore.api.Dtos;

public record GameDto(
    int Id,
    string Name,
    string? GenreName,
    decimal Price,
    DateOnly? ReleaseDate
    );