using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using icestats_api.Data;
using icestats_api.Models;

namespace icestats_api.Controllers;

/// <summary>
/// The GamesController handles all HTTP requests related to hockey games.
///
/// API Routes:
/// - GET /api/games        -> Get all games
/// - GET /api/games/{id}   -> Get a specific game by ID
/// - POST /api/games       -> Create a new game
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IceStatsDbContext _context;

    public GamesController(IceStatsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/games
    /// Returns a list of all games in the database.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames()
    {
        var games = await _context.Games.ToListAsync();
        return Ok(games);
    }

    /// <summary>
    /// GET: api/games/{id}
    /// Returns a specific game by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    /// <summary>
    /// POST: api/games
    /// Creates a new game in the database.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(Game game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }
}