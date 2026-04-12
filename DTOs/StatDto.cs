namespace icestats_api.DTOs;

/// <summary>
/// Stat DTO - represents a player's statistics for a specific game.
/// This is what we send when returning game stats from our API.
/// </summary>
public class StatDto
{
    // Unique identifier for this stat record
    public int Id { get; set; }

    // Player information
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }

    // Game information
    public int GameId { get; set; }
    public string OpponentTeam { get; set; } = string.Empty;
    public DateTime GameDate { get; set; }

    // Performance statistics for this game
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int PenaltyMinutes { get; set; }
    public int PlusMinus { get; set; }
}