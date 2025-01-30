using System.Text.Json;
using Confluent.Kafka;
using RateLimiter.Reader.Models;

namespace RateLimiter.Reader.Kafka;

public class KafkaConsumer
{
    private readonly ConsumerConfig _config;
    private readonly KafkaMapper _mapper;
    private readonly string _topic;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(string broker, string topic, KafkaMapper mapper)
    {
        _mapper = mapper;
        _config = new ConsumerConfig
        {
            BootstrapServers = broker,
            GroupId = "reader",
            SecurityProtocol = SecurityProtocol.Plaintext,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        _topic = topic;
        _logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<KafkaConsumer>();
    }

    public async IAsyncEnumerable<UserRequest> ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler((_, error) => _logger.LogError($"Error: {error}"))
            .Build();

        try
        {
            consumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = consumer.Consume(cancellationToken);
                if (result is null)
                {
                    continue;
                }

                var message = JsonSerializer.Deserialize<KafkaMessage>(result.Message.Value);
                if (message is not null)
                {
                    yield return _mapper.KafkaMessageToUserRequest(message);
                }
            }
        }
        finally
        {
            consumer.Close();
            consumer.Dispose();
        }
    }
}