using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI;

var builder = WebApplication.CreateBuilder(args);

// ── CORS Configuration ──────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendUrl = builder.Configuration["FRONTEND_URL"] 
            ?? builder.Configuration["Cors:AllowedOrigins"];
            
        if (!string.IsNullOrEmpty(frontendUrl))
        {
            var origins = frontendUrl.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// ── Database Configuration ──────────────────────────
string GetFormattedConnectionString(IConfiguration config)
{
    var rawConnection = config["DATABASE_URL"] 
        ?? config.GetConnectionString("Supabase") 
        ?? config.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(rawConnection)) return rawConnection ?? "";

    if (rawConnection.StartsWith("postgres://") || rawConnection.StartsWith("postgresql://"))
    {
        var uri = new Uri(rawConnection);
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
    return rawConnection;
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(GetFormattedConnectionString(builder.Configuration)));

builder.Services.AddScoped<OutReachToursAPI.Services.IEmailService, OutReachToursAPI.Services.SmtpEmailService>();
builder.Services.AddScoped<OutReachToursAPI.Services.IPaymentService, OutReachToursAPI.Services.PaystackPaymentService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Automatically apply pending migrations
    context.Database.Migrate();

    // Seed mock data
    OutReachToursAPI.DataSeeder.SeedData(context);

    // Seed admin user
    var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin@outreachtours.com");
    if (adminUser == null)
    {
        context.Users.Add(new OutReachToursAPI.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Admin User",
            Email = "admin@outreachtours.com",
            PasswordHash = OutReachToursAPI.Controllers.AuthController.ComputeHash("password123"),
            RoleId = "admin_role_id"
        });
    }
    else
    {
        // Force update the existing admin user to have the correct role and password
        adminUser.RoleId = "admin_role_id";
        
        adminUser.PasswordHash = OutReachToursAPI.Controllers.AuthController.ComputeHash("password123");
    }
    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");

// Health check endpoint for Railway monitoring
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow }));

// Map controllers
app.MapControllers();

app.Run();


