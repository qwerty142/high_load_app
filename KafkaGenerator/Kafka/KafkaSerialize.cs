using System.Text.Json;
using Confluent.Kafka;

namespace KafkaGenerator.Kafka;

public class KafkaSerialize : ISerializer<KafkaMessage>
{
    public byte[] Serialize(KafkaMessage data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}