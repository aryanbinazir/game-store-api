namespace GameStore.api.Dtos;

public record GameDto(
    int Id,
    string Name,
    int IdGenre,
    decimal Price,
    DateOnly ReleaseDate
    );