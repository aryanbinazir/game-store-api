using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Dtos;

public record CreateGameDto(
    [Required][MaxLength(50)]string Name,
    [Required][MaxLength(30)]string Genre,
    [Range(typeof(decimal), "1", "100")]
    decimal Price,
    DateOnly ReleaseDate
    );