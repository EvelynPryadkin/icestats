using System.Text.Json.Serialization;

namespace icestats_api.Models.NHL;

/// <summary>
/// Represents an NHL skater (forward or defenseman) and their statistics.
/// This class is used to deserialize JSON from the NHL API endpoint:
/// https://api.nhle.com/stats/rest/en/skater/summary
///
/// Example JSON structure (actual NHL API response):
/// {
///     "playerId": 8471234,
///     "skaterFullName": "Connor McDavid",
///     "positionCode": "C",
///     "teamAbbrevs": "EDM",
///     "seasonId": 20242025,
///     "gamesPlayed": 82,
///     "goals": 44,
///     "assists": 71,
///     "points": 115
/// }
///
/// The NHL API uses different property names than our C# models, so we use
/// [JsonPropertyName] attributes to map them correctly.
/// </summary>
public class NhlSkater
{
    /// <summary>
    /// NHL player ID - unique identifier for the player
    /// We'll use this as our primary key.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Full player name from API (e.g., "Connor McDavid")
    /// Stored directly - no parsing needed for database storage
    /// </summary>
    [JsonPropertyName("skaterFullName")]
    public string? SkaterFullName { get; set; }

    /// <summary>
    /// Player's position code: C (Center), RW, LW, D (Defense)
    /// </summary>
    [JsonPropertyName("positionCode")]
    public string? Position { get; set; }

    /// <summary>
    /// Team abbreviation(s) - comma separated if player was traded
    /// </summary>
    [JsonPropertyName("teamAbbrevs")]
    public string? TeamAbbreviations { get; set; }

    /// <summary>
    /// Season ID in format YYYYYYYY (e.g., 20242025 = 2024-25 season)
    /// </summary>
    [JsonPropertyName("seasonId")]
    public int SeasonId { get; set; }

    /// <summary>
    /// Number of games the player participated in this season
    /// </summary>
    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Total goals scored by this player
    /// </summary>
    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    /// <summary>
    /// Total assists made by this player
    /// </summary>
    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    /// <summary>
    /// Total points (Goals + Assists)
    /// </summary>
    [JsonPropertyName("points")]
    public int Points { get; set; }

    /// <summary>
    /// Plus/Minus rating
    /// </summary>
    [JsonPropertyName("plusMinus")]
    public int PlusMinus { get; set; }

    /// <summary>
    /// Penalty minutes
    /// </summary>
    [JsonPropertyName("penaltyMinutes")]
    public int PenaltyMinutes { get; set; }

    /// <summary>
    /// Power play goals
    /// </summary>
    [JsonPropertyName("ppGoals")]
    public int PowerPlayGoals { get; set; }

    /// <summary>
    /// Power play points
    /// </summary>
    [JsonPropertyName("ppPoints")]
    public int PowerPlayPoints { get; set; }

    /// <summary>
    /// Short-handed goals
    /// </summary>
    [JsonPropertyName("shGoals")]
    public int ShortHandedGoals { get; set; }

    /// <summary>
    /// Short-handed points
    /// </summary>
    [JsonPropertyName("shPoints")]
    public int ShortHandedPoints { get; set; }

    /// <summary>
    /// Game-winning goals
    /// </summary>
    [JsonPropertyName("gameWinningGoals")]
    public int GameWinningGoals { get; set; }

    /// <summary>
    /// Overtime goals
    /// </summary>
    [JsonPropertyName("otGoals")]
    public int OverTimeGoals { get; set; }

    /// <summary>
    /// Shots on goal
    /// </summary>
    [JsonPropertyName("shots")]
    public int Shots { get; set; }

    /// <summary>
    /// Shooting percentage
    /// </summary>
    [JsonPropertyName("shootingPct")]
    public double? ShotPercentage { get; set; }

    /// <summary>
    /// Time on ice per game (in seconds)
    /// </summary>
    [JsonPropertyName("timeOnIcePerGame")]
    public double? TimeOnIcePerGame { get; set; }

    /// <summary>
    /// When this skater data was retrieved from the API
    /// </summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Helper property to get first name (computed at runtime)
    /// Note: This is NOT mapped to the database - only used in the controller
    /// </summary>
    [JsonIgnore]
    public string? FirstName => SkaterFullName?.Split(' ')[0];

    /// <summary>
    /// Helper property to get last name (computed at runtime)
    /// Note: This is NOT mapped to the database - only used in the controller
    /// </summary>
    [JsonIgnore]
    public string? LastName => 
        SkaterFullName != null && SkaterFullName.Split(' ').Length > 1 
            ? string.Join(" ", SkaterFullName.Split(' ')[1..]) 
            : null;

    /// <summary>
    /// Helper property to get full name (computed at runtime)
    /// </summary>
    [JsonIgnore]
    public string FullName => SkaterFullName ?? "";

    /// <summary>
    /// Helper property to get team name from abbreviation
    /// Note: This is NOT mapped to the database - only used in the controller
    /// </summary>
    [JsonIgnore]
    public string TeamName => GetTeamNameFromAbbreviation(TeamAbbreviations?.Split(',')[0]);

    /// <summary>
    /// Helper method to get team name from abbreviation (public for use in controller)
    /// Note: This is NOT mapped to the database
    /// </summary>
    public static string GetTeamNameFromAbbreviation(string? abbrev)
    {
        return NhlGoalLeader.GetTeamNameFromAbbreviation(abbrev);
    }
}