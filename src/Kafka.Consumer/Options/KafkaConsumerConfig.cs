namespace Kafka.Consumer.Options;

public class KafkaConsumerConfig
{
    public required IEnumerable<string> Topics { get; init; }
    public string TopicsAsSingleString => string.Join(",", Topics);
    [Required]
    public string BootstrapServers { get; init; } = null!;
    [Required] 
    public string ClientId { get; init; } = null!;
    [Required]
    public string GroupId { get; init; } = null!;
    [Required]
    public string SecurityProtocol { get; init; } = null!;
    public int BatchSize { get; init; } = 1;
    public int BatchIntervalInSeconds { get; init; } = 1;
}