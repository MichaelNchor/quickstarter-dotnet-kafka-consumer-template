namespace Kafka.Consumer.Models;

public record KafkaMessage : BaseModel
{
    [JsonConstructor]
    protected KafkaMessage(string message)
    {
        Message = message;
    }
    public string? Message { get; set; }
}