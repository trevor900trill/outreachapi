using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Assuming default Next.js port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Supabase")));

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
app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Note: You must apply EF Migrations before this will run successfully.
app.Run();

