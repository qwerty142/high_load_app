using MongoDB.Driver;
using RateLimiter.Reader.Controllers;
using RateLimiter.Reader.Kafka;
using RateLimiter.Reader.Repositories;
using RateLimiter.Reader.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

// repository
builder.Services.AddSingleton<ILimiterRepository, LimiterRepository>();
builder.Services.AddSingleton<IMongoClient>(_ 
    => new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

// service
builder.Services.AddSingleton<ILimiterService, LimiterService>();

// kafka
builder.Services.AddSingleton<KafkaMapper>();
builder.Services.AddSingleton<KafkaConsumer>(x 
    => new KafkaConsumer(
            builder.Configuration.GetSection("Kafka").GetSection("BootstrapServers").Value!,
            builder.Configuration.GetSection("Kafka").GetSection("Topic").Value!,
            x.GetRequiredService<KafkaMapper>()
        ));
builder.Services.AddSingleton<IRestrictionRepository, RedisRepository>();

// redis
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => 
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

// hosted service
builder.Services.AddHostedService<BackgroundLimiterLoadService>();
builder.Services.AddHostedService<RestrictionService>();

var app = builder.Build();

app.MapGrpcService<ReaderController>();
await app.RunAsync("http://*:5003");