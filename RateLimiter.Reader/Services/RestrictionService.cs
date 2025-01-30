using System.Collections.Concurrent;
using System.Text.Json;
using RateLimiter.Reader.Kafka;
using RateLimiter.Reader.Repositories;
using StackExchange.Redis;

namespace RateLimiter.Reader.Services;

public class RestrictionService : IHostedService
{
    private const int BanDurationMinutes = 5;
    
    private readonly ConcurrentDictionary<int, ConcurrentQueue<DateTime>> _userRequests;
    private readonly KafkaConsumer _consumer;
    private readonly ILimiterService _limiterService;
    private readonly IRestrictionRepository _restrictionRepository;
    private readonly ILogger<RestrictionService> _logger;

    public RestrictionService(
        KafkaConsumer consumer,
        ILimiterService limiterService,
        IRestrictionRepository restrictionRepository)
    {
        _consumer = consumer;
        _limiterService = limiterService;
        _restrictionRepository = restrictionRepository;
        _userRequests = new ConcurrentDictionary<int, ConcurrentQueue<DateTime>>();
        _logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<RestrictionService>();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ProcessRequests(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ProcessRequests(CancellationToken cancellationToken)
    {
        await foreach (var request in _consumer.ConsumeMessagesAsync(cancellationToken))
        {
            if (await _restrictionRepository.IsRestrictedAsync(request.UserId, request.Endpoint))
            {
                _logger.LogWarning($"User {request.UserId} restricted on {request.Endpoint}. Request denied.");
                continue;
            }
            
            if (_userRequests.ContainsKey(request.UserId))
            {
                var currentRequests = _userRequests[request.UserId];
                while (!currentRequests.IsEmpty && currentRequests.TryPeek(out var time))
                {
                    if (time < DateTime.Now.AddMinutes(-1))
                    {
                        currentRequests.TryDequeue(out _);
                    }
                    else
                    {
                        break;
                    }
                }

                var result = _limiterService.GetLimitByRoute(request.Endpoint);
                if (result.IsFailed)
                {
                    _logger.LogInformation($"Request from {request.UserId} to {request.Endpoint} processed.");
                    continue;
                }
                
                var limit = result.Value.Limit;
                var rpm = currentRequests.Count;
                if (rpm < limit)
                {
                    currentRequests.Enqueue(DateTime.Now);
                    _logger.LogInformation($"Request from {request.UserId} to {request.Endpoint} processed.");
                }
                else
                {
                    await _restrictionRepository.SetRestrictionAsync(
                        request.UserId,
                        request.Endpoint,
                        TimeSpan.FromMinutes(BanDurationMinutes));
                }
            }
            else
            {
                var queue = new ConcurrentQueue<DateTime>();
                _userRequests[request.UserId] = queue;
                queue.Enqueue(DateTime.Now);
            }
        }
    }
}