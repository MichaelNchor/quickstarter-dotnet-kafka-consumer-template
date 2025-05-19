namespace Kafka.Consumer.Services.Interface;

public interface IKafkaConsumerService
{
    Task Consume(CancellationToken ctx);
}