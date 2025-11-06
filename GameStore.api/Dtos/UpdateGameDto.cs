using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GameStore.api.Dtos;

public record UpdateGameDto(   
    [Required][MaxLength(50)]string Name,
    [Required][MaxLength(30)]string Genre,
    [Range(typeof(decimal), "1", "100")]
    decimal Price,
    DateOnly ReleaseDate
    );