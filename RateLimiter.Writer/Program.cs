using MongoDB.Driver;
using RateLimiter.Writer.Controllers;
using RateLimiter.Writer.Repositories;
using RateLimiter.Writer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddScoped<IRateLimiterRepository, RateLimiterRepository>();
builder.Services.AddScoped<IRateLimiterService, RateLimiterService>();
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.MapGrpcService<RateLimiterController>();

await app.RunAsync("http://localhost:5001");