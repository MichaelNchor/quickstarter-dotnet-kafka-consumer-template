namespace Kafka.Consumer.Services.Interface;

public interface IElasticClientService
{
    Task Add(object message, CancellationToken ctx);
}