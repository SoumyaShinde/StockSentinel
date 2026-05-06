using StockSentinal.Data;
using Microsoft.EntityFrameworkCore;
using StockSentinal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using StockSentinal.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IWatchlistService, WatchlistService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:SecretKey"]!
                )
            )
        };
    });

builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Add Hangfire
builder.Services.AddHangfire(config => config.UseInMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddScoped<StockPricePollingJob>();

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockSentinel API v1");
    c.RoutePrefix = string.Empty; // ← makes Swagger load at root URL!
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Use Hangfire dashboard
app.UseHangfireDashboard();

// Schedule recurring job — every 1 minute
RecurringJob.AddOrUpdate<StockPricePollingJob>(
    "stock-price-polling",
    job => job.Execute(),
    "*/5 * * * *"  // cron = every 5 minute
);

app.Run();