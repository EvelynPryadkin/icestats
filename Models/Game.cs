using System.ComponentModel.DataAnnotations;

namespace icestats_api.Models;

/// <summary>
/// The Game model represents a hockey game.
/// Each game has two teams (home and away) and stores final scores.
/// </summary>
public class Game
{
    // Primary Key - unique identifier for each game
    [Key]
    public int Id { get; set; }

    // Home team name
    [Required]
    [MaxLength(50)]
    public string HomeTeam { get; set; } = string.Empty;

    // Away team name
    [Required]
    [MaxLength(50)]
    public string AwayTeam { get; set; } = string.Empty;

    // Final score for the home team
    [Required]
    public int HomeScore { get; set; }

    // Final score for the away team
    [Required]
    public int AwayScore { get; set; }

    // Date and time when the game was played
    [Required]
    public DateTime GameDate { get; set; }

    // Location of the game (arena/name)
    [MaxLength(100)]
    public string? Location { get; set; }

    // When this game record was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property: a game can have many player stats
    public ICollection<PlayerStat> PlayerStats { get; set; } = new List<PlayerStat>();
}