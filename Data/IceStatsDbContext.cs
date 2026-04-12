using Microsoft.EntityFrameworkCore;
using icestats_api.Models;
using icestats_api.Models.NHL;

namespace icestats_api.Data;

/// <summary>
/// The DbContext is Entity Framework Core's main connection between your C# code and the MySQL database.
/// It acts as a bridge that:
/// 1. Maps your C# classes (Models) to database tables
/// 2. Tracks changes made to data in memory
/// 3. Sends SQL commands to the database when you call SaveChanges()
///
/// The name "IceStatsDbContext" follows convention - DbContext suffix is standard.
/// </summary>
public class IceStatsDbContext : DbContext
{
    // Constructor - receives configuration options from Program.cs
    public IceStatsDbContext(DbContextOptions<IceStatsDbContext> options) 
        : base(options)
    {
    }

    // DbSets represent tables in the database.
    // Entity Framework Core will create these tables based on your Models.
    //
    // Players table - stores all hockey players
    public DbSet<Player> Players => Set<Player>();

    // Games table - stores all game records
    public DbSet<Game> Games => Set<Game>();

    // PlayerStats table - stores player statistics for each game (many-to-many relationship)
    public DbSet<PlayerStat> PlayerStats => Set<PlayerStat>();

    // NHL Tables - for storing real NHL data synced from the API
    public DbSet<NhlStanding> NhlStandings => Set<NhlStanding>();
    public DbSet<NhlSkater> NhlSkaters => Set<NhlSkater>();
    public DbSet<NhlGoalLeader> NhlGoalLeaders => Set<NhlGoalLeader>();

    /// <summary>
    /// This method is optional - used to configure the database schema in more detail.
    /// You can override it to customize column names, data types, indexes, etc.
    /// </summary>
    /// <param name="modelBuilder">The builder that creates the database model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Player entity
        modelBuilder.Entity<Player>()
            .Property(p => p.Name)
            .IsRequired();

        modelBuilder.Entity<Player>()
            .Property(p => p.Position)
            .IsRequired();

        // Configure Game entity
        modelBuilder.Entity<Game>()
            .Property(g => g.HomeTeam)
            .IsRequired();

        modelBuilder.Entity<Game>()
            .Property(g => g.AwayTeam)
            .IsRequired();

        // Configure PlayerStat entity (many-to-many join table)
        modelBuilder.Entity<PlayerStat>()
            .HasOne(ps => ps.Player)
            .WithMany()
            .HasForeignKey(ps => ps.PlayerId);

        modelBuilder.Entity<PlayerStat>()
            .HasOne(ps => ps.Game)
            .WithMany(g => g.PlayerStats)
            .HasForeignKey(ps => ps.GameId);

        // Configure NHL entities
        modelBuilder.Entity<NhlStanding>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<NhlSkater>()
            .HasKey(s => s.PlayerId);
        // Note: FirstName, LastName are computed from SkaterFullName
        // TeamName is computed from TeamAbbreviations - these don't have backing fields
        // so we ignore them and configure the actual API properties only
        modelBuilder.Entity<NhlSkater>()
            .Ignore(s => s.FirstName)
            .Ignore(s => s.LastName);
        modelBuilder.Entity<NhlSkater>()
            .Property(s => s.TeamAbbreviations);

        modelBuilder.Entity<NhlGoalLeader>()
            .HasKey(l => l.PlayerId);
        // Note: FirstName, LastName are direct properties in NhlGoalLeader (no computation)
        modelBuilder.Entity<NhlGoalLeader>()
            .Ignore(g => g.FullName);
        modelBuilder.Entity<NhlGoalLeader>()
            .Property(g => g.TeamAbbreviation);
    }
}
