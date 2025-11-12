using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Entities;

public class Game
{
    // Properties
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    public decimal Price { get; set; } = -1;
    public DateOnly? ReleaseDate { get; set; }
    
    // Relations
    public int? GenreId { get; set; } 
    public Genre? Genre { get; set; }
}