namespace Kafka.Consumer.Services.Interface;

public interface IElasticRepository<in T> where T : class
{
    Task Add(T message, CancellationToken ctx);
}