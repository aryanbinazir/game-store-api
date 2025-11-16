using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Dtos.Game;

public record CreateGameDto(
    [Required][MaxLength(50)]
    string Name,
    int? IdGenre,
    [Range(typeof(decimal), "1", "100")]
    decimal Price,
    [Range(typeof(DateOnly), "1958-01-01", "2040-12-30")]
    DateOnly? ReleaseDate
    );