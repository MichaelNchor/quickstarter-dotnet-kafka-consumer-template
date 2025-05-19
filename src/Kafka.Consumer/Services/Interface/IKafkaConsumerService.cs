namespace Kafka.Consumer.Services.Interface;

public interface IKafkaConsumerService
{
    Task StartConsumer(CancellationToken ctx);
}