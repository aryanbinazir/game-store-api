using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GameStore.api.Dtos;

public record UpdateGameDto(   
    [MaxLength(50)] string? Name,
    int? IdGenre,
    [Range(typeof(decimal), "1", "100")] decimal? Price,
    DateOnly? ReleaseDate
    );