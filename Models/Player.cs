using System.ComponentModel.DataAnnotations;

namespace icestats_api.Models;

/// <summary>
/// The Player model represents a hockey player in our database.
/// Each property becomes a column in the "players" table in MySQL.
/// This is called an Entity Framework Core Model or Domain Model.
/// </summary>
public class Player
{
    // Primary Key - unique identifier for each player
    [Key]
    public int Id { get; set; }

    // The player's full name (required field)
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Player number on their team
    [Required]
    public int JerseyNumber { get; set; }

    // Position: Forward, Defense, or Goaltender
    [Required]
    [MaxLength(20)]
    public string Position { get; set; } = string.Empty;

    // Team the player plays for
    [MaxLength(50)]
    public string? Team { get; set; }

    // When this player record was created (automatically set by database)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}