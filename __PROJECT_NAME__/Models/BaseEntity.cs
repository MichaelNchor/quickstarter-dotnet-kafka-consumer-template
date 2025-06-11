namespace __PROJECT_NAME__.Models;

public record BaseEntity
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}