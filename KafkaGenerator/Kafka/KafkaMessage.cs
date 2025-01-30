namespace KafkaGenerator.Kafka;

public class KafkaMessage
{
    public KafkaMessage(int id, string endpoint)
    {
        this.user_id = id;
        this.endpoint = endpoint;
    }
    public int user_id { get; set; }
    public string endpoint { get; set; }
}