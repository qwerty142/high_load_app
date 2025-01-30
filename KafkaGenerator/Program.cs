using KafkaGenerator;
using KafkaGenerator.Kafka;
using KafkaGenerator.Models;

var kafkaBroker = "localhost:9092";
var topic = "user-events";;
var producerApp = new KafkaProducer(kafkaBroker, topic);

Console.WriteLine("Kafka Generator started");
Console.WriteLine("Commands:\nset <user_id> <endpoint> <rpm>\ndelete <user_id> [endpoint]\nexit");
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Trim().ToLower().Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    try
    {
        var tokens = input.Split(" ");
        if (tokens[0].Equals("set", StringComparison.OrdinalIgnoreCase))
        {
            var userId = int.Parse(tokens[1]);
            var endpoint = tokens[2];
            var rpm = int.Parse(tokens[3]);
            producerApp.AddOrUpdateUser(new EventConfig(userId, endpoint, rpm));
        }
        else if (tokens[0].Equals("delete", StringComparison.OrdinalIgnoreCase))
        {
            var userId = int.Parse(tokens[1]);
            if (tokens.Length == 2)
            {
                producerApp.RemoveUser(userId);
            }
            else
            {
                var endpoint = tokens[2];
                producerApp.RemoveUserEndpoint(userId, endpoint);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n{ex.Message}\n");
    }
}