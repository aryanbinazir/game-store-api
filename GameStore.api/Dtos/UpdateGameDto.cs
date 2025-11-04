namespace GameStore.api.Dtos;

public record UpdateGameDto(   
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
    );