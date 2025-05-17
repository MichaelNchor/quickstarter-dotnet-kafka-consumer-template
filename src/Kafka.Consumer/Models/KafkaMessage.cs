namespace Kafka.Consumer.Models;

public record KafkaMessage
{
    public KafkaMessage(string message)
    {
        Id = Guid.NewGuid().ToString();
        Message = message;
        Timestamp = DateTime.UtcNow;
    }
    public string? Id { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}