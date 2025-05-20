

namespace Kafka.Consumer;

public class KafkaConsumer : KafkaConsumerBase
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IElasticRepository<KafkaMessage> _elasticRepository;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IElasticRepository<KafkaMessage> elasticRepository)
    {
        _logger = logger;
        _elasticRepository = elasticRepository;
    }

    [Consume(Type = typeof(IOptions<KafkaConsumerConfig>), Topics = nameof(KafkaConsumerConfig.TopicsAsSingleString))]
    private async Task HandleKafkaMessages(List<KafkaMessage> messages)
    {
        _logger.LogInformation("Kafka messages received...");
        foreach (var message in messages)
        {
            try
            {
                await _elasticRepository.Add(message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to index message: {message} and exception: {exception} @ {timestamp}",
                    JsonConvert.SerializeObject(message), ex.Message, DateTime.UtcNow);
            }
        }
    }
}