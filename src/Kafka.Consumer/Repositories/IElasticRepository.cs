#if UseOpenSearch
namespace Kafka.Consumer.Repositories;

public interface IElasticRepository<in T> where T : class
{
    Task Add(T message, CancellationToken ctx);
}
#endif