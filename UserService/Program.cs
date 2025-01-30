using Npgsql;
using StackExchange.Redis;
using UserService.Controllers;
using UserService.Controllers.Interceptors;
using UserService.Entities.Validation;
using UserService.Models.Mappers;
using UserService.Repositories;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// data
builder.Services.AddScoped<IUserRepository, UserRepository>();
await using var source = NpgsqlDataSource.Create(builder.Configuration.GetConnectionString("PostgreSQL")!);
builder.Services.AddSingleton(source);

// redis
builder.Services.AddSingleton<IConnectionMultiplexer>(_ 
    => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

// service
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddSingleton<UserValidator>();
builder.Services.AddSingleton<UserDbMapper>();
builder.Services.AddSingleton<UserGrpcMapper>();
builder.Services.AddSingleton<IRestrictionService, RedisRestrictionService>();

// cache
builder.Services.AddMemoryCache();

// gRPC
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<UserIdHeaderInterceptor>();
    options.Interceptors.Add<UserRateLimitInterceptor>();
});

var app = builder.Build();
app.MapGrpcService<UserController>();

await app.RunAsync("http://localhost:5002");