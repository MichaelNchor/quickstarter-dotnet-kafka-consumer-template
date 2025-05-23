namespace Kafka.Consumer.Consumers;

public class AccountingConsumer : KafkaConsumerBase
{
    private readonly ILogger<AccountingConsumer> _logger;
    private readonly IElasticRepository<AccountMessage> _elasticRepository;

    public AccountingConsumer(ILogger<AccountingConsumer> logger, IElasticRepository<AccountMessage> elasticRepository)
    {
        _logger = logger;
        _elasticRepository = elasticRepository;
    }

    [Consume(Type = typeof(KafkaExtraConfig), Property = nameof(KafkaExtraConfig.KafkaTopic2))]
    private async Task HandleKafkaMessages(List<AccountMessage> messages)
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