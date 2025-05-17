using System.Text.Json.Serialization;
using Kafka.Consumer.Models;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace Kafka.Consumer.Services;

public class ElasticClientService : IElasticClientService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticClientService> _logger;
    public ElasticClientService(IElasticClient elasticClient, ILogger<ElasticClientService> logger)
    {
        _logger = logger;
        _elasticClient = elasticClient;
    }
    
    public async Task IndexRecord<T>(T message, CancellationToken ctx) where T : KafkaMessage
    {
        var indexResponse = await _elasticClient.IndexDocumentAsync(message, ctx);
        if (indexResponse.IsValid)
        {
            _logger.LogInformation("Successfully indexed message : {message} @ {timestamp}", 
                JsonConvert.SerializeObject(message), DateTime.UtcNow);
        }
        else
        {
            _logger.LogError("Failed to index message: {message} and exception: {exception} @ {timestamp}", 
                JsonConvert.SerializeObject(message), indexResponse.OriginalException.Message, DateTime.UtcNow);
        }    
    }
}