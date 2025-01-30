using System.Collections.Concurrent;
using Confluent.Kafka;
using KafkaGenerator.Models;
using Microsoft.Extensions.Logging;

namespace KafkaGenerator.Kafka;

public class KafkaProducer
{
    private readonly string _topic;
    private readonly ConcurrentDictionary<int, ConcurrentBag<(EventConfig config, CancellationTokenSource cts)>> _userTasks;
    private readonly ProducerConfig _config;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(string kafkaBroker, string topic)
    {
        _config = new ProducerConfig
        {
            BootstrapServers = kafkaBroker,
        };
        _topic = topic;
        _userTasks = new ConcurrentDictionary<int, ConcurrentBag<(EventConfig config, CancellationTokenSource cts)>>();
        _logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<KafkaProducer>();

    }

    public void AddOrUpdateUser(EventConfig config)
    {
        RemoveUserEndpoint(config.UserId, config.Endpoint);

        var cts = new CancellationTokenSource();
        var newBag = _userTasks.GetOrAdd(config.UserId, _ => []);
        newBag.Add((config, cts));
        
        _logger.LogInformation($"added: {config.UserId} to endpoint {config.Endpoint} and rpm {config.Rpm}");
        Task.Run(() => StartSendingEvents(config, cts.Token));
    }

    public void RemoveUserEndpoint(int userId, string endpoint)
    {
        if (_userTasks.TryGetValue(userId, out var bag))
        {
            var existingTask = bag.FirstOrDefault((x) => x.config.Endpoint.Equals(endpoint));
            if (!existingTask.Equals(default))
            {
                existingTask.cts.Cancel();
                var list = bag.ToList();
                list.RemoveAll(x => x.config.Endpoint.Equals(endpoint));
                _userTasks[userId] = new ConcurrentBag<(EventConfig config, CancellationTokenSource cts)>(list);
                _logger.LogInformation($"Removed endpoint {endpoint} of user {userId}");
            }
        }
    }

    public void RemoveUser(int userId)
    {
        if (_userTasks.TryRemove(userId, out var task))
        {
            var list = task.ToList();
            _logger.LogInformation($"Removed endpoints of user {userId}");
            list.ForEach(x => x.cts.Cancel());
        }
        else
        {
            _logger.LogError($"fail while removing {userId}");
        }
    }

    private async Task StartSendingEvents(EventConfig config, CancellationToken token)
    {
        var delay = TimeSpan.FromMinutes(1) / config.Rpm;
        using var producer = new ProducerBuilder<string, KafkaMessage>(_config)
            .SetErrorHandler((_, error) => _logger.LogError($"Error: {error}"))
            .SetValueSerializer(new KafkaSerialize())
            .Build();
        
        while (!token.IsCancellationRequested)
        {
            var message = new KafkaMessage(config.UserId, config.Endpoint);
            
            var kafkaMessage = new Message<string, KafkaMessage>
            {
                Key = config.UserId.ToString(),
                Value = message
            };

            await producer.ProduceAsync(_topic, kafkaMessage);
            // _logger.LogInformation($"Sent: {message.user_id} to {message.endpoint}");
            await Task.Delay(delay, token);
        }
    }
}