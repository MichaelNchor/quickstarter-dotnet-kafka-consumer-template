namespace Kafka.Consumer.Models;

public abstract class KafkaConsumerBase
{
    public KafkaContext Context { get; set; } = null!;
}

public class KafkaContext(List<ConsumeResult<Null, string>> messageBag, string bootstrapServers)
{
    public List<ConsumeResult<Null, string>> MessageBagBag { get; } = messageBag;

    public string BootstrapServers { get; } = bootstrapServers;
}