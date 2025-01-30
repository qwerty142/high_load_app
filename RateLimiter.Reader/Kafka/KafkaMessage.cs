namespace RateLimiter.Reader.Kafka;

public record KafkaMessage(int user_id, string endpoint);