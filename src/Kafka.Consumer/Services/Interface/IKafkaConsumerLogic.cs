namespace Kafka.Consumer.Services.Interface;

public interface IKafkaConsumerLogic
{
    Task StartConsumer(CancellationToken ctx);
}