namespace Kafka.Consumer.Options;

public class KafkaConfig
{
    [Required] 
    public KafkaConsumerConfig Consumer { get; init; } = null!;
}

public class KafkaConsumerConfig
{
    [Required] 
    [MinLength(1, ErrorMessage = "At least one topic is required.")]
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