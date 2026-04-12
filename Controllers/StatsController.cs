using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using icestats_api.Data;
using icestats_api.Models;
using icestats_api.DTOs;

namespace icestats_api.Controllers;

/// <summary>
/// The StatsController handles all HTTP requests related to player statistics.
/// This includes viewing stats for specific games or calculating career totals.
///
/// API Routes:
/// - GET /api/stats                    -> Get all stats
/// - GET /api/stats/player/{playerId}  -> Get stats for a specific player
/// - GET /api/stats/game/{gameId}      -> Get stats for a specific game
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IceStatsDbContext _context;

    public StatsController(IceStatsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/stats
    /// Returns all player statistics.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatDto>>> GetStats()
    {
        var stats = await _context.PlayerStats
            .Select(s => new StatDto
            {
                Id = s.Id,
                PlayerId = s.PlayerId,
                PlayerName = s.Player.Name,
                JerseyNumber = s.Player.JerseyNumber,
                GameId = s.GameId,
                OpponentTeam = s.Game.HomeTeam == s.Player.Team ? s.Game.AwayTeam : s.Game.HomeTeam,
                GameDate = s.Game.GameDate,
                Goals = s.Goals,
                Assists = s.Assists,
                PenaltyMinutes = s.PenaltyMinutes,
                PlusMinus = s.PlusMinus
            })
            .ToListAsync();

        return Ok(stats);
    }

    /// <summary>
    /// GET: api/stats/player/{playerId}
    /// Returns all statistics for a specific player (their career stats).
    /// </summary>
    [HttpGet("player/{playerId}")]
    public async Task<ActionResult<IEnumerable<StatDto>>> GetPlayerStats(int playerId)
    {
        var stats = await _context.PlayerStats
            .Where(s => s.PlayerId == playerId)
            .Select(s => new StatDto
            {
                Id = s.Id,
                PlayerId = s.PlayerId,
                PlayerName = s.Player.Name,
                JerseyNumber = s.Player.JerseyNumber,
                GameId = s.GameId,
                OpponentTeam = s.Game.HomeTeam == s.Player.Team ? s.Game.AwayTeam : s.Game.HomeTeam,
                GameDate = s.Game.GameDate,
                Goals = s.Goals,
                Assists = s.Assists,
                PenaltyMinutes = s.PenaltyMinutes,
                PlusMinus = s.PlusMinus
            })
            .ToListAsync();

        return Ok(stats);
    }

    /// <summary>
    /// GET: api/stats/game/{gameId}
    /// Returns all statistics for a specific game.
    /// </summary>
    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<IEnumerable<StatDto>>> GetGameStats(int gameId)
    {
        var stats = await _context.PlayerStats
            .Where(s => s.GameId == gameId)
            .Select(s => new StatDto
            {
                Id = s.Id,
                PlayerId = s.PlayerId,
                PlayerName = s.Player.Name,
                JerseyNumber = s.Player.JerseyNumber,
                GameId = s.GameId,
                OpponentTeam = s.Game.HomeTeam == s.Player.Team ? s.Game.AwayTeam : s.Game.HomeTeam,
                GameDate = s.Game.GameDate,
                Goals = s.Goals,
                Assists = s.Assists,
                PenaltyMinutes = s.PenaltyMinutes,
                PlusMinus = s.PlusMinus
            })
            .ToListAsync();

        return Ok(stats);
    }
}