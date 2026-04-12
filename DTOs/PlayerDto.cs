namespace icestats_api.DTOs;

/// <summary>
/// Player DTO (Data Transfer Object) - what we send to clients.
/// This is a simplified version of the Player model that only includes
/// the data we want to show API users (not database internals like Id).
///
/// Why use DTOs?
/// 1. Security: Don't expose internal IDs or sensitive data
/// 2. Flexibility: Can combine data from multiple models
/// 3. Performance: Send only the fields needed
/// 4. Stability: API contract doesn't break if database changes
/// </summary>
public class PlayerDto
{
    // Unique identifier for the player (for API interactions)
    public int Id { get; set; }

    // The player's full name
    public string Name { get; set; } = string.Empty;

    // Player number on their team
    public int JerseyNumber { get; set; }

    // Position: Forward, Defense, or Goaltender
    public string Position { get; set; } = string.Empty;

    // Team the player plays for
    public string? Team { get; set; }

    // Total games played (calculated field)
    public int GamesPlayed { get; set; }

    // Total career goals (calculated field)
    public int TotalGoals { get; set; }

    // Total career assists (calculated field)
    public int TotalAssists { get; set; }
}