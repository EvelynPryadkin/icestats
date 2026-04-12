using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using icestats_api.Data;
using icestats_api.Models;
using icestats_api.DTOs;

namespace icestats_api.Controllers;

/// <summary>
/// The PlayersController handles all HTTP requests related to hockey players.
/// 
/// API Routes:
/// - GET /api/players        -> Get all players
/// - GET /api/players/{id}   -> Get a specific player by ID
/// - POST /api/players       -> Create a new player
/// - PUT /api/players/{id}   -> Update an existing player
/// - DELETE /api/players/{id}-> Delete a player
///
/// [ApiController] - Tells ASP.NET Core this is an API controller (enables automatic HTTP response handling)
/// [Route("api/[controller]")] - All routes will start with "/api/players"
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IceStatsDbContext _context;

    /// <summary>
    /// Constructor receives the database context through Dependency Injection.
    /// ASP.NET Core automatically provides the configured DbContext.
    /// </summary>
    public PlayersController(IceStatsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/players
    /// Returns a list of all players in the database.
    /// This is what you'll see when you visit localhost:5000/api/players in your browser!
    /// </summary>
    /// <returns>A list of PlayerDto objects</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
    {
        // Query the database for all players
        var players = await _context.Players.ToListAsync();

        // Convert Player entities to PlayerDto objects (what we send to clients)
        var playerDtos = players.Select(p => new PlayerDto
        {
            Id = p.Id,
            Name = p.Name,
            JerseyNumber = p.JerseyNumber,
            Position = p.Position,
            Team = p.Team,
            GamesPlayed = 0, // Will calculate in a real implementation
            TotalGoals = 0,
            TotalAssists = 0
        }).ToList();

        return Ok(playerDtos);
    }

    /// <summary>
    /// GET: api/players/{id}
    /// Returns a specific player by their ID.
    /// </summary>
    /// <param name="id">The unique ID of the player</param>
    /// <returns>The PlayerDto object for the requested player</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
        {
            return NotFound(); // Returns 404 Not Found
        }

        var playerDto = new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            JerseyNumber = player.JerseyNumber,
            Position = player.Position,
            Team = player.Team,
            GamesPlayed = 0,
            TotalGoals = 0,
            TotalAssists = 0
        };

        return Ok(playerDto);
    }

    /// <summary>
    /// POST: api/players
    /// Creates a new player in the database.
    /// </summary>
    /// <param name="player">The Player object to create</param>
    /// <returns>The created PlayerDto</returns>
    [HttpPost]
    public async Task<ActionResult<Player>> CreatePlayer(Player player)
    {
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
    }
}