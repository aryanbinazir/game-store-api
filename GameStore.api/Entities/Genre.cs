using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Entities;

public class Genre
{
    // Properties
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    
    // Relations 
    public List<Game>? Games { get; set; }
    
}