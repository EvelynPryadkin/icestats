# IceStats - Hockey Statistics REST API

A hockey statistics REST API built with ASP.NET Core Web API, C#, and MySQL using Entity Framework Core. Features real-time NHL data synchronization from the official NHL API.

## Project Structure

```
icestats-api/
├── Controllers/           # API endpoints (handles HTTP requests)
│   ├── PlayersController.cs    # Player CRUD operations
│   ├── GamesController.cs      # Game CRUD operations  
│   ├── StatsController.cs      # Statistics operations
│   └── SyncController.cs       # NHL data synchronization endpoints
├── Models/                # Database table definitions (entities)
│   ├── Player.cs             # Represents a hockey player
│   ├── Game.cs               # Represents a hockey game
│   ├── PlayerStat.cs         # Represents player stats for a specific game
│   └── NHL/                  # NHL API data models (read-only synced data)
│       ├── NhlSkater.cs      # Skater statistics from NHL API
│       ├── NhlGoalLeader.cs  # Goal scoring leaders from NHL API
│       └── NhlStanding.cs    # Team standings from NHL API
├── Data/                  # Entity Framework Core configuration
│   └── IceStatsDbContext.cs  # Database context and model configuration
├── DTOs/                  # Data Transfer Objects (API responses)
│   ├── PlayerDto.cs
│   └── StatDto.cs
├── Services/              # Business logic layer
│   └── NhlSyncService.cs     # Handles API calls to NHL and saves data
├── Program.cs             # Application entry point and DI configuration
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

### NHL Data Synchronization (SyncController)
- `POST /api/sync/skaters` - Sync top 50 skaters by points from NHL API
- `POST /api/sync/goalleaders` - Sync top 20 goal scorers from NHL API
- `GET /api/nhl/leaders/skaters?limit=50` - Get cached skater leaderboard data
- `GET /api/nhl/leaders/goals?limit=20` - Get cached goal leader data

## Prerequisites

- .NET 10.0 SDK or later
- MySQL Server running on localhost
- Visual Studio Code (recommended)
- For Angular frontend: Node.js and npm installed

## Setup Instructions (Backend)

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

The API will start on `https://localhost:7001` and `http://localhost:5048`.

### 5. Sync NHL Data

Call the sync endpoints to fetch live data from the NHL API:

```bash
# Sync top 50 skaters by points
curl -X POST http://localhost:5048/api/sync/skaters

# Sync top 20 goal scorers  
curl -X POST http://localhost:5048/api/sync/goalleaders

# View the data
curl http://localhost:5048/api/nhl/leaders/skaters
curl http://localhost:5048/api/nhl/leaders/goals
```

## Technology Stack

- **Language**: C# 12
- **Framework**: ASP.NET Core Web API (.NET 10)
- **Database**: MySQL 8.0+
- **ORM**: Entity Framework Core 8.0 with Pomelo provider
- **API Documentation**: OpenAPI/Swagger
- **Frontend (optional)**: Angular with TypeScript

## CORS Configuration for Angular Frontend

If building an Angular frontend, CORS is configured to allow requests from `http://localhost:4200` (Angular's default dev server port). The API will respond with:
```
Access-Control-Allow-Origin: http://localhost:4200
```

To use the frontend, run `ng serve --port 4200` in the icestats-frontend directory.

## How This Project Works (Interview Talking Points)

### Models vs DTOs
- **Models** are database entities - they represent tables in MySQL and include all internal properties needed for data persistence
- **DTOs** (Data Transfer Objects) are what we send to API clients - they hide internal implementation details and only expose necessary fields

### Controller Pattern
- Controllers receive HTTP requests and return appropriate responses
- `[ApiController]` enables automatic model validation and consistent HTTP response formatting
- `[Route("api/[controller]")]` creates RESTful routes automatically (e.g., PlayersController → /api/players)
- Dependency injection allows us to inject services like NhlSyncService via constructor

### Entity Framework Core
- DbContext connects C# code to MySQL database - acts as a bridge between OOP and relational databases
- DbSets represent database tables (one per entity type)
- Migrations allow us to version-control our database schema and apply changes incrementally
- Code-first approach: we define models in C#, EF generates the SQL schema

### Service Pattern (NhlSyncService)
- Business logic is separated from controllers into dedicated service classes
- HttpClientFactory manages HTTP client lifecycle efficiently
- Dependency injection registers services so any controller can use them

### NHL API Integration
- SyncController endpoints trigger data pulls from official NHL stats API
- Data is cached in MySQL for faster frontend queries
- Separate models (NhlSkater, NhlGoalLeader) store read-only synced data