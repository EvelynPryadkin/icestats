using System.Text.Json.Serialization;

namespace icestats_api.Models.NHL;

/// <summary>
/// Represents an NHL player who is a leader in goals.
/// This class is used to deserialize JSON from the NHL API endpoint:
/// https://api-web.nhle.com/v1/skater-stats-leaders/{season}/{gameType}
///
/// Example JSON structure (simplified):
/// {
///     "playerId": 8471234,
///     "firstName": {
///         "default": "Connor"
///     },
///     "lastName": {
///         "default": "McDavid"
///     },
///     "position": "C",
///     "teamAbbrev": "EDM",
///     "gamesPlayed": 82,
///     "goals": 44
/// }
/// </summary>
public class NhlGoalLeader
{
    /// <summary>
    /// NHL player ID - unique identifier for the player
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// First name of the player (default language)
    /// The NHL API returns nested objects, but we'll use strings directly for EF compatibility
    /// </summary>
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of the player (default language)
    /// </summary>
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    /// <summary>
    /// Full skater name (NHL API returns "skaterFullName" like "Nathan MacKinnon")
    /// This takes precedence over FirstName + LastName combination
    /// </summary>
    [JsonPropertyName("skaterFullName")]
    public string? SkaterFullName { get; set; }

    /// <summary>
    /// Player's position: C, RW, LW, D
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    /// <summary>
    /// Team 3-letter abbreviation (NHL API returns "teamAbbrevs" in the JSON)
    /// </summary>
    [JsonPropertyName("teamAbbrevs")]
    public string? TeamAbbreviation { get; set; }

    /// <summary>
    /// Number of games played
    /// </summary>
    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Total goals scored by this player (this is what we're ranking by)
    /// </summary>
    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    /// <summary>
    /// Total assists
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
    [JsonPropertyName("pim")]
    public int PenaltyMinutes { get; set; }

    /// <summary>
    /// Power play goals
    /// </summary>
    [JsonPropertyName("powerPlayGoals")]
    public int PowerPlayGoals { get; set; }

    /// <summary>
    /// Game-winning goals
    /// </summary>
    [JsonPropertyName("gameWinningGoals")]
    public int GameWinningGoals { get; set; }

    /// <summary>
    /// Overtime goals
    /// </summary>
    [JsonPropertyName("overTimeGoals")]
    public int OverTimeGoals { get; set; }

    /// <summary>
    /// Shots on goal
    /// </summary>
    [JsonPropertyName("shots")]
    public int Shots { get; set; }

    /// <summary>
    /// Shooting percentage
    /// </summary>
    [JsonPropertyName("shotPctg")]
    public double? ShotPercentage { get; set; }

    /// <summary>
    /// When this leader data was retrieved from the API
    /// </summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Helper property to get full player name
    /// Uses SkaterFullName if available, otherwise combines FirstName + LastName
    /// </summary>
    [JsonIgnore]
    public string FullName => !string.IsNullOrEmpty(SkaterFullName) 
        ? SkaterFullName 
        : $"{FirstName ?? ""} {LastName ?? ""}".Trim();

    /// <summary>
    /// Helper property to get team name (using abbreviation)
    /// </summary>
    [JsonIgnore]
    public string TeamName => GetTeamNameFromAbbreviation(TeamAbbreviation);

    /// <summary>
    /// Helper method to get team name from abbreviation
    /// </summary>
    public static string GetTeamNameFromAbbreviation(string? abbrev)
    {
        return abbrev?.ToUpperInvariant() switch
        {
            "ANA" => "Anaheim Ducks",
            "ARI" => "Arizona Coyotes",
            "BOS" => "Boston Bruins",
            "BUF" => "Buffalo Sabres",
            "CGY" => "Calgary Flames",
            "CAR" => "Carolina Hurricanes",
            "CHI" => "Chicago Blackhawks",
            "COL" => "Colorado Avalanche",
            "CBJ" => "Columbus Blue Jackets",
            "DAL" => "Dallas Stars",
            "DET" => "Detroit Red Wings",
            "EDM" => "Edmonton Oilers",
            "FLA" => "Florida Panthers",
            "LAK" => "Los Angeles Kings",
            "MIN" => "Minnesota Wild",
            "MTL" => "Montreal Canadiens",
            "NJD" => "New Jersey Devils",
            "NSH" => "Nashville Predators",
            "NYI" => "New York Islanders",
            "NYR" => "New York Rangers",
            "OTT" => "Ottawa Senators",
            "PHI" => "Philadelphia Flyers",
            "PIT" => "Pittsburgh Penguins",
            "SJS" => "San Jose Sharks",
            "SEA" => "Seattle Kraken",
            "STL" => "St. Louis Blues",
            "TBL" => "Tampa Bay Lightning",
            "TOR" => "Toronto Maple Leafs",
            "VAN" => "Vancouver Canucks",
            "VGK" => "Vegas Golden Knights",
            "WSH" => "Washington Capitals",
            "WPG" => "Winnipeg Jets",
            _ => abbrev ?? "Unknown"
        };
    }
}