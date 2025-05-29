namespace Kafka.Consumer.Options;

public class OpenSearchConfig
{
    [Required]
    [Url(ErrorMessage = "Uri must be a valid URL.")]
    public string Uri { get; init; } = null!;

    [Required]
    public string IndexName { get; init; } = null!;
}