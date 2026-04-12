# IceStats - Hockey Statistics REST API

A hockey statistics REST API built with ASP.NET Core Web API, C#, and MySQL using Entity Framework Core.

## Project Structure

```
icestats-api/
├── Controllers/           # API endpoints (handles HTTP requests)
│   ├── PlayersController.cs
│   ├── GamesController.cs
│   └── StatsController.cs
├── Models/                # Database table definitions (entities)
│   ├── Player.cs          # Represents a hockey player
│   ├── Game.cs            # Represents a hockey game
│   └── PlayerStat.cs      # Represents player stats for a specific game
├── Data/                  # Entity Framework Core configuration
│   └── IceStatsDbContext.cs
├── DTOs/                  # Data Transfer Objects (API responses)
│   ├── PlayerDto.cs
│   └── StatDto.cs
├── Program.cs             # Application entry point
└── appsettings.json       # Configuration (database connection string)
```

## API Endpoints

### Players
- `GET /api/players` - Get all players
- `GET /api/players/{id}` - Get a specific player by ID
- `POST /api/players` - Create a new player

### Games
- `GET /api/games` - Get all games
- `GET /api/games/{id}` - Get a specific game by ID
- `POST /api/games` - Create a new game

### Stats
- `GET /api/stats` - Get all player statistics
- `GET /api/stats/player/{playerId}` - Get stats for a specific player
- `GET /api/stats/game/{gameId}` - Get stats for a specific game

## Prerequisites

- .NET 10.0 SDK or later
- MySQL Server running on localhost
- Visual Studio Code (recommended)

## Setup Instructions

### 1. Create the Database

Open MySQL and create the database:
```sql
CREATE DATABASE icestats_db;
```

### 2. Update Connection String

Edit `appsettings.json` and update the connection string with your MySQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=icestats_db;uid=your_username;pwd=your_password;"
}
```

### 3. Create Database Migrations

Run these commands in order:

```bash
# Create the initial migration (creates database schema)
dotnet ef migrations add InitialCreate

# Apply migrations to create/update the database
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The API will start on `https://localhost:7001` and `http://localhost:5001`.

### 5. Test the API

Open your browser and navigate to:
- `https://localhost:7001/api/players` - Should return an empty list initially
- `http://localhost:5001/swagger` - OpenAPI/Swagger documentation (if available)

## Technology Stack

- **Language**: C# 12
- **Framework**: ASP.NET Core Web API (.NET 10)
- **Database**: MySQL 8.0+
- **ORM**: Entity Framework Core 8.0 with Pomelo provider
- **API Documentation**: OpenAPI/Swagger

## How This Project Works (Interview Talking Points)

### Models vs DTOs
- **Models** are database entities - they represent tables in MySQL
- **DTOs** (Data Transfer Objects) are what we send to API clients - they hide internal implementation details

### Controller Pattern
- Controllers receive HTTP requests and return responses
- `[ApiController]` enables automatic model validation and HTTP response formatting
- `[Route("api/[controller]")]` creates RESTful routes automatically

### Entity Framework Core
- DbContext connects C# code to MySQL database
- DbSets represent database tables
- Migrations allow us to version-control our database schema

## License

MIT