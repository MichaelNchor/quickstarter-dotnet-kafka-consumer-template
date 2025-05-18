namespace Kafka.Consumer.Helpers;

public abstract class KafkaBase
{
    public KafkaContext Context { get; set; } = null!;
}

public class KafkaContext(List<ConsumeResult<Null, string>> messages, string bootstrapServers)
{
    public List<ConsumeResult<Null, string>> MessageBag { get; } = messages;

    public string BootstrapServers { get; } = bootstrapServers;
}