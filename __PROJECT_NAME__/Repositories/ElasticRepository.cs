#if UseOpenSearch

namespace __PROJECT_NAME__.Repositories;

public class ElasticRepository<T> : IElasticRepository<T> where T : class
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticRepository<T>> _logger;

    public ElasticRepository(ILogger<ElasticRepository<T>> logger, IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task Add(T message, CancellationToken ctx)
    {
        var indexResponse = await _elasticClient.IndexDocumentAsync(message, ctx);
        if (indexResponse.IsValid)
        {
            _logger.LogInformation("Successfully indexed message : {message} at {timestamp}", 
                JsonConvert.SerializeObject(message), DateTime.UtcNow);
        }
        else
        {
            _logger.LogError("Failed to index message: {message} and exception: {exception} at {timestamp}", 
                JsonConvert.SerializeObject(message), indexResponse.OriginalException.Message, DateTime.UtcNow);
        }    
    }
}

#endif