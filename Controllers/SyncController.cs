using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using icestats_api.Data;
using icestats_api.Models.NHL;
using icestats_api.Services;

namespace icestats_api.Controllers;

/// <summary>
/// The SyncController handles syncing real NHL data from the NHL API to our local MySQL database.
///
/// Why a separate controller for sync?
/// -----------------------------------
/// We separate sync operations into their own controller because:
/// 1. They have different purposes (data ingestion vs. querying)
/// 2. Different security considerations (POST vs GET)
/// 3. Easier to maintain and extend
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly NhlSyncService _syncService;

    /// <summary>
    /// Constructor - receives the sync service via Dependency Injection.
    /// ASP.NET Core automatically provides a configured NhlSyncService instance.
    /// </summary>
    public SyncController(NhlSyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// POST: api/sync/standings
    /// Fetches current NHL team standings and saves them to our database.
    /// This is a POST because it modifies data in the database.
    /// </summary>
    [HttpPost("standings")]
    public async Task<ActionResult> SyncStandings()
    {
        var success = await _syncService.SyncStandingsAsync();
        
        if (success)
        {
            return Ok(new { message = "Successfully synced NHL standings!" });
        }
        
        return StatusCode(500, new { message = "Failed to sync standings. Check server logs." });
    }

    /// <summary>
    /// POST: api/sync/skaters
    /// Fetches top 50 NHL skaters by points and saves them to our database.
    /// </summary>
    [HttpPost("skaters")]
    public async Task<ActionResult> SyncSkaters()
    {
        var success = await _syncService.SyncSkatersAsync();
        
        if (success)
        {
            return Ok(new { message = "Successfully synced NHL skaters!" });
        }
        
        return StatusCode(500, new { message = "Failed to sync skaters. Check server logs." });
    }

    /// <summary>
    /// POST: api/sync/teams
    /// Fetches current NHL team standings and saves them to our database.
    /// </summary>
    [HttpPost("teams")]
    public async Task<ActionResult> SyncTeams()
    {
        var success = await _syncService.SyncTeamsAsync();
        
        if (success)
        {
            return Ok(new { message = "Successfully synced NHL teams!" });
        }
        
        return StatusCode(500, new { message = "Failed to sync teams. Check server logs." });
    }

    /// <summary>
    /// POST: api/sync/goalleaders
    /// Fetches top 20 NHL goal scorers and saves them to our database.
    /// </summary>
    [HttpPost("goalleaders")]
    public async Task<ActionResult> SyncGoalLeaders()
    {
        var success = await _syncService.SyncGoalLeadersAsync();
        
        if (success)
        {
            return Ok(new { message = "Successfully synced NHL goal leaders!" });
        }
        
        return StatusCode(500, new { message = "Failed to sync goal leaders. Check server logs." });
    }
}

/// <summary>
/// The NhlController handles querying the NHL data we've synced to our database.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NhlController : ControllerBase
{
    private readonly IceStatsDbContext _context;

    public NhlController(IceStatsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/nhl/standings
    /// Returns all NHL team standings from our local database.
    /// </summary>
    [HttpGet("standings")]
    public async Task<ActionResult<IEnumerable<object>>> GetStandings()
    {
        var standings = await _context.NhlStandings
            .Select(s => new
            {
                s.Id,
                TeamName = s.TeamName ?? "Unknown",
                s.Abbreviation,
                GamesPlayed = s.GamesPlayed,
                Wins = s.Wins,
                Losses = s.Losses,
                OvertimeLosses = s.OvertimeLosses,
                Points = s.Points
            })
            .OrderByDescending(s => s.Points)
            .ToListAsync();

        return Ok(standings);
    }

     /// <summary>
     /// GET: api/nhl/leaders/goals
     /// Returns top goal scorers from our local database.
     /// </summary>
     [HttpGet("leaders/goals")]
     public async Task<ActionResult<IEnumerable<object>>> GetGoalLeaders()
     {
         var leaders = await _context.NhlGoalLeaders
             .Select(l => new
             {
                 l.PlayerId,
                 FullName = l.FullName,
                 l.Position,
                 TeamName = l.TeamAbbreviation ?? "Unknown",
                 GamesPlayed = l.GamesPlayed,
                 Goals = l.Goals,
                 Assists = l.Assists,
                 Points = l.Points
             })
             .OrderByDescending(l => l.Goals)
             .Take(20)
             .ToListAsync();

         return Ok(leaders);
     }

    /// <summary>
    /// GET: api/nhl/teams
    /// Returns all NHL teams from our local database.
    /// </summary>
    [HttpGet("teams")]
    public async Task<ActionResult<IEnumerable<object>>> GetTeams()
    {
        var teams = await _context.NhlTeams
            .Select(t => new
            {
                t.TeamAbbrev,
                TeamFullName = t.TeamName ?? "Unknown",
                t.TeamLogo,
                Wins = t.Wins,
                Losses = t.Losses,
                OtLosses = t.OtLosses,
                Points = t.Points,
                DivisionName = t.DivisionName,
                ConferenceName = t.ConferenceName,
                StreakCode = t.StreakCode
            })
            .OrderByDescending(t => t.Points)
            .ToListAsync();

        return Ok(teams);
    }

    /// <summary>
    /// GET: api/nhl/leaders/skaters
    /// Returns top skaters by points from our local database.
    /// </summary>
    [HttpGet("leaders/skaters")]
    public async Task<ActionResult<IEnumerable<object>>> GetSkaterLeaders()
    {
        var leaders = await _context.NhlSkaters
            .Take(50)
            .Select(s => new
            {
                s.PlayerId,
                FullName = s.FullName,
                s.Position,
                TeamName = s.TeamName ?? "Unknown",
                GamesPlayed = s.GamesPlayed,
                Goals = s.Goals,
                Assists = s.Assists,
                Points = s.Points,
                PlusMinus = s.PlusMinus
            })
            .OrderByDescending(s => s.Points)
            .ToListAsync();

        return Ok(leaders);
    }
}