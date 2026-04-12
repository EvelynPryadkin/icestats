using Microsoft.EntityFrameworkCore;
using icestats_api.Data;
using icestats_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS to allow requests from Angular frontend (localhost:4200)
// This is needed because the browser enforces same-origin policy by default
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        // Allow specific origin - this matches Angular's default dev server port
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()   // Allow all HTTP methods (GET, POST, PUT, DELETE)
               .AllowAnyHeader();  // Allow any headers to be sent
    });
});

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

// Enable CORS for the Angular frontend
// This must be called before MapControllers() to work properly
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
