namespace Kafka.Consumer.Services;

public interface IKafkaConsumerLogic
{
    Task StartConsumingAsync(CancellationToken ctx);
    void Dispose();
}