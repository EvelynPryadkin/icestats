using System.ComponentModel.DataAnnotations;

namespace icestats_api.Models;

/// <summary>
/// The PlayerStat model represents a player's performance statistics in a specific game.
/// This creates a many-to-many relationship between Players and Games.
/// For example: "John Smith scored 2 goals in Game #10".
/// </summary>
public class PlayerStat
{
    // Primary Key - unique identifier for each stat record
    [Key]
    public int Id { get; set; }

    // Foreign Key to the Player table (which player)
    [Required]
    public int PlayerId { get; set; }

    // Foreign Key to the Game table (which game)
    [Required]
    public int GameId { get; set; }

    // Goals scored by this player in this game
    [Required]
    public int Goals { get; set; }

    // Assists made by this player in this game
    [Required]
    public int Assists { get; set; }

    // Penalty minutes for this player in this game
    public int PenaltyMinutes { get; set; }

    // Plus/Minus rating (+1 for each goal scored by team, -1 for each goal against)
    public int PlusMinus { get; set; }

    // When this stat record was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Player Player { get; set; } = new Player();
    public Game Game { get; set; } = new Game();
}