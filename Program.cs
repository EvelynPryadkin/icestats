using Microsoft.EntityFrameworkCore;
using icestats_api.Data;
using icestats_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure HttpClient for NHL API calls
// This registers a named HttpClient that we can inject into our service
builder.Services.AddHttpClient<NhlSyncService>(client =>
{
    // The base address will be overridden in each method call since we use full URLs
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configure Entity Framework Core with MySQL database
// Read connection string from appsettings.json
builder.Services.AddDbContext<IceStatsDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Register the NhlSyncService for dependency injection
// This allows controllers to receive it via constructor injection
builder.Services.AddScoped<NhlSyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();