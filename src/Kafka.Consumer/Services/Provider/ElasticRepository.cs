namespace Kafka.Consumer.Services.Provider;

public class ElasticRepository(IElasticClient elasticClient, ILogger<ElasticRepository> logger) : IElasticRepository
{
    public async Task Add<T>(T message, CancellationToken ctx) where T : BaseEntity
    {
        var indexResponse = await elasticClient.IndexDocumentAsync(message, ctx);
        if (indexResponse.IsValid)
        {
            logger.LogInformation("Successfully indexed message : {message} @ {timestamp}", 
                JsonConvert.SerializeObject(message), DateTime.UtcNow);
        }
        else
        {
            logger.LogError("Failed to index message: {message} and exception: {exception} @ {timestamp}", 
                JsonConvert.SerializeObject(message), indexResponse.OriginalException.Message, DateTime.UtcNow);
        }    
    }
}