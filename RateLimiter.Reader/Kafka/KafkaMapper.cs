using RateLimiter.Reader.Models;
using Riok.Mapperly.Abstractions;

namespace RateLimiter.Reader.Kafka;

public class KafkaMapper
{
    public UserRequest KafkaMessageToUserRequest(KafkaMessage kafkaMessage)
    {
        return new UserRequest(kafkaMessage.user_id, kafkaMessage.endpoint);
    }
}