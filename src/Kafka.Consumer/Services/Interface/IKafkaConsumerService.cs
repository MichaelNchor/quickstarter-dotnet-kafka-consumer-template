namespace Kafka.Consumer.Services.Interface;

public interface IKafkaConsumerService
{
    Task StartConsumingAsync(CancellationToken ctx);
    void Dispose();
}