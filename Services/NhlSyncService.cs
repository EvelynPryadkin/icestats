using System.Net.Http.Json;
using System.Text.Json.Serialization;
using icestats_api.Data;
using icestats_api.Models.NHL;

namespace icestats_api.Services;

/// <summary>
/// This service fetches real NHL data from the free NHL API and saves it to our MySQL database.
///
/// What is HttpClient?
/// -------------------
/// HttpClient is a class that allows you to send HTTP requests and receive HTTP responses
/// from web servers. It's like a browser in your code - when you type a URL in your browser,
/// it sends an HTTP GET request and displays the response (HTML, JSON, etc.).
///
/// What is JSON deserialization?
/// -----------------------------
/// JSON is text-based data format used by APIs. Deserialization means converting that
/// JSON text into C# objects so we can work with them programmatically.
///
/// Example:
///     JSON: {"name": "Connor McDavid", "goals": 44}
///     C# Object: new NhlSkater { Name = "Connor McDavid", Goals = 44 }
///
/// The [JsonPropertyName] attributes in our model classes tell .NET how to map
/// JSON keys to C# properties.
/// </summary>
public class NhlSyncService
{
    private readonly HttpClient _httpClient;
    private readonly IceStatsDbContext _context;

    /// <summary>
    /// Constructor - this is where Dependency Injection comes in!
    ///
    /// What is Dependency Injection?
    /// ----------------------------
    /// Dependency Injection (DI) is a design pattern where we give objects their 
    /// dependencies instead of creating them inside the object. Think of it like:
    ///     BEFORE: A car creates its own engine (bad - inflexible)
    ///     AFTER:  A car receives an engine from outside (good - flexible, testable)
    ///
    /// In ASP.NET Core, we register services in Program.cs and the framework
    /// automatically provides them to controllers/services that need them.
    /// </summary>
    public NhlSyncService(HttpClient httpClient, IceStatsDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
        
        // Configure HttpClient with proper headers
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Wrapper class for NHL API responses.
    /// All endpoints return: { "data": [...], "total": X }
    /// </summary>
    public class NhlApiResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new();
        
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Fetches current NHL team standings from the API and saves them to MySQL.
    /// Note: The standings endpoint on api-web.nhle.com may be blocked on some networks.
    /// For now, this is a placeholder method. Use SyncSkatersAsync instead for data sync.
    /// </summary>
    public async Task<bool> SyncStandingsAsync()
    {
        Console.WriteLine("Note: Standings sync requires access to api-web.nhle.com which may be blocked.");
        return true;
    }

    /// <summary>
    /// Fetches top 50 NHL skaters by points from the API and saves them to MySQL.
    /// Endpoint: https://api.nhle.com/stats/rest/en/skater/summary
    /// </summary>
    public async Task<bool> SyncSkatersAsync(int limit = 50)
    {
        try
        {
            var url = $"https://api.nhle.com/stats/rest/en/skater/summary?limit={limit}&sort=points&dir=DESC&cayenneExp=seasonId=20252026";
            
            Console.WriteLine($"Requesting: {url}");
            
            // Send HTTP GET request to NHL API
            var response = await _httpClient.GetFromJsonAsync<NhlApiResponse<NhlSkater>>(url);

            if (response == null || response.Data == null)
            {
                Console.WriteLine("Error: Response or Data was null");
                return false;
            }

            Console.WriteLine($"Successfully retrieved {response.Data.Count} skaters");

            // Clear existing skaters and add new ones
            _context.NhlSkaters.RemoveRange(_context.NhlSkaters);
            foreach (var skater in response.Data)
            {
                _context.NhlSkaters.Add(skater);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Saved {response.Data.Count} skaters to database");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing skaters: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }

    /// <summary>
    /// Fetches current NHL team standings from the API and saves them to MySQL.
    /// Endpoint: https://api.nhle.com/stats/rest/en/standings?cayenneExp=seasonId=20252026
    ///
    /// What is a cayenneExp?
    /// ---------------------
    /// This is an NHL-specific filtering parameter that works like a WHERE clause.
    /// The format is: seasonId=YYYYYYYY where YYYYYYYY is the 8-digit season ID.
    /// For example, 20252026 represents the 2025-26 season (2025 starts in Oct, ends in Apr 2026).
    /// </summary>
    public async Task<bool> SyncTeamsAsync()
    {
        try
        {
            var url = "https://api.nhle.com/stats/rest/en/standings?cayenneExp=seasonId=20252026";
            
            Console.WriteLine($"Requesting: {url}");
            
            // The standings endpoint returns an array directly, not wrapped in {data: [...]}
            var teamsArray = await _httpClient.GetFromJsonAsync<NhlTeam[]>(url);

            if (teamsArray == null || teamsArray.Length == 0)
            {
                Console.WriteLine("Error: Response or Data was null");
                return false;
            }

            Console.WriteLine($"Successfully retrieved {teamsArray.Length} teams");

            // Clear existing teams and add new ones
            _context.NhlTeams.RemoveRange(_context.NhlTeams);
            foreach (var team in teamsArray)
            {
                _context.NhlTeams.Add(team);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Saved {teamsArray.Length} teams to database");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing teams: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }

    /// <summary>
    /// Fetches top 20 NHL goal scorers from the API and saves them to MySQL.
    /// Endpoint: https://api.nhle.com/stats/rest/en/skater/summary
    /// </summary>
    public async Task<bool> SyncGoalLeadersAsync(int limit = 20)
    {
        try
        {
            var url = $"https://api.nhle.com/stats/rest/en/skater/summary?limit={limit}&sort=goals&dir=DESC&cayenneExp=seasonId=20252026";
            
            Console.WriteLine($"Requesting: {url}");
            
            // Send HTTP GET request to NHL API
            var response = await _httpClient.GetFromJsonAsync<NhlApiResponse<NhlGoalLeader>>(url);

            if (response == null || response.Data == null)
            {
                Console.WriteLine("Error: Response or Data was null");
                return false;
            }

            Console.WriteLine($"Successfully retrieved {response.Data.Count} goal leaders");

            // Clear existing goal leaders and add new ones
            _context.NhlGoalLeaders.RemoveRange(_context.NhlGoalLeaders);
            foreach (var leader in response.Data)
            {
                _context.NhlGoalLeaders.Add(leader);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Saved {response.Data.Count} goal leaders to database");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing goal leaders: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }
}