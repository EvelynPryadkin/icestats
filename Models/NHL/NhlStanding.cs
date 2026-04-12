using System.Text.Json.Serialization;

namespace icestats_api.Models.NHL;

/// <summary>
/// Represents a team's standing in the NHL.
/// This class is used to deserialize JSON from the NHL API endpoint:
/// https://api-web.nhle.com/v1/standings/now
///
/// Example JSON structure (simplified):
/// {
///     "id": 24,
///     "teamName": {
///         "default": "Toronto Maple Leafs"
///     },
///     "abbreviation": "TOR",
///     "leagueAbbrev": "NHL",
///     "gamePlayed": 50,
///     "wins": 32,
///     "losses": 14,
///     "otLosses": 4,
///     "points": 68
/// }
/// </summary>
public class NhlStanding
{
    /// <summary>
    /// NHL team ID (e.g., 24 for Toronto Maple Leafs)
    /// We'll use this as our primary key.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Team's full name in default language
    /// Example: "Toronto Maple Leafs"
    /// </summary>
    [JsonPropertyName("teamName")]
    public string? TeamName { get; set; }

    /// <summary>
    /// Team's 3-letter abbreviation (e.g., TOR, BOS, NYR)
    /// </summary>
    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; set; }

    /// <summary>
    /// League abbreviation - always "NHL" for NHL teams
    /// </summary>
    [JsonPropertyName("leagueAbbrev")]
    public string? LeagueAbbrev { get; set; }

    /// <summary>
    /// Number of games the team has played
    /// </summary>
    [JsonPropertyName("gamePlayed")]
    public int GamesPlayed { get; set; }

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
    public int OvertimeLosses { get; set; }

    /// <summary>
    /// Total points (calculated as: wins * 2 + otLosses * 1)
    /// In the NHL, a win = 2 points, OT loss = 1 point, regulation loss = 0
    /// </summary>
    [JsonPropertyName("points")]
    public int Points { get; set; }

    /// <summary>
    /// Team's position in their division
    /// </summary>
    [JsonPropertyName("divisionRank")]
    public string? DivisionRank { get; set; }

    /// <summary>
    /// League-wide rank based on points
    /// </summary>
    [JsonPropertyName("leagueRank")]
    public string? LeagueRank { get; set; }

    /// <summary>
    /// Points percentage (points / max possible points)
    /// </summary>
    [JsonPropertyName("ptsPctg")]
    public double? PointsPercentage { get; set; }

    /// <summary>
    /// When this standing was retrieved from the API
    /// </summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Team name object - contains translations for different languages
/// The NHL API returns team names in multiple languages.
/// </summary>
public class NhlTeamName
{
    [JsonPropertyName("default")]
    public string? Default { get; set; }

    [JsonPropertyName("fr")]
    public string? French { get; set; }

    [JsonPropertyName("en")]
    public string? English { get; set; }
}