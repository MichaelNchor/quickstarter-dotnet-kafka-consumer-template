namespace Kafka.Consumer.Models;

public record BaseModel
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}