#if UseOpenSearch
namespace __PROJECT_NAME__.Repositories;

public interface IElasticRepository<in T> where T : class
{
    Task Add(T message, CancellationToken ctx);
}
#endif