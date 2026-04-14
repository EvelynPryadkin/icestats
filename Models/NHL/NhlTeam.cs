using System.Text.Json.Serialization;

namespace icestats_api.Models.NHL;

/// <summary>
/// Represents an NHL team's standing and record.
/// This class is used to deserialize JSON from the NHL API endpoint:
/// https://api.nhle.com/stats/rest/en/standings?cayenneExp=seasonId=20252026
///
/// Example JSON structure (simplified):
/// {
///     "teamName": {
///         "default": "Toronto Maple Leafs"
///     },
///     "teamAbbrev": "TOR",
///     "wins": 32,
///     "losses": 14,
///     "otLosses": 4,
///     "points": 68,
///     "divisionName": "Atlantic",
///     "conferenceName": "Eastern"
/// }
/// </summary>
public class NhlTeam
{
    /// <summary>
    /// Team's full name in default language (e.g., "Toronto Maple Leafs")
    /// </summary>
    [JsonPropertyName("teamName")]
    public string? TeamName { get; set; }

    /// <summary>
    /// Team's 3-letter abbreviation (e.g., TOR, BOS, NYR)
    /// We'll use this as our primary key.
    /// </summary>
    [JsonPropertyName("teamAbbrev")]
    public string? TeamAbbrev { get; set; }

    /// <summary>
    /// URL to team logo image
    /// </summary>
    [JsonPropertyName("teamLogo")]
    public string? TeamLogo { get; set; }

    /// <summary>
    /// Number of wins (regulation + overtime)
    /// </summary>
    [JsonPropertyName("wins")]
    public int Wins { get; set; }

    /// <summary>
    /// Number of losses (regulation only)
    /// </summary>
    [JsonPropertyName("losses")]
    public int Losses { get; set; }

    /// <summary>
    /// Number of overtime/shootout losses
    /// </summary>
    [JsonPropertyName("otLosses")]
    public int OtLosses { get; set; }

    /// <summary>
    /// Total points (calculated as: wins * 2 + otLosses * 1)
    /// In the NHL, a win = 2 points, OT loss = 1 point, regulation loss = 0
    /// </summary>
    [JsonPropertyName("points")]
    public int Points { get; set; }

    /// <summary>
    /// Team's division name (e.g., "Atlantic", "Metropolitan")
    /// </summary>
    [JsonPropertyName("divisionName")]
    public string? DivisionName { get; set; }

    /// <summary>
    /// Team's conference name ("Eastern" or "Western")
    /// </summary>
    [JsonPropertyName("conferenceName")]
    public string? ConferenceName { get; set; }

    /// <summary>
    /// Current streak code (e.g., W3 for 3 wins, L2 for 2 losses)
    /// </summary>
    [JsonPropertyName("streakCode")]
    public string? StreakCode { get; set; }

    /// <summary>
    /// When this team data was retrieved from the API
    /// </summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}