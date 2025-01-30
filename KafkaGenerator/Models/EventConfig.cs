namespace KafkaGenerator.Models;

public class EventConfig
{
    public EventConfig(int userId, string endpoint, int rpm)
    {
        UserId = userId;
        Endpoint = endpoint;
        Rpm = rpm;
    }
    public int UserId { get; set; }
    public string Endpoint { get; set; }
    public int Rpm { get; set; }
}