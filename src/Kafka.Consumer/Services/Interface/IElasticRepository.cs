namespace Kafka.Consumer.Services.Interface;

public interface IElasticRepository
{
    Task Add<T>(T message, CancellationToken ctx) where T : BaseEntity;
}