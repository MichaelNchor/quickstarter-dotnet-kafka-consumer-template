namespace Kafka.Consumer.Services.Interface;

public interface IKafkaConsumerLogic
{
    Task StartConsumingAsync(CancellationToken ctx);
    void Dispose();
}