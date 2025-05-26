namespace Kafka.Consumer.Consumers;

public class AccountingConsumer : KafkaConsumerBase
{
    private readonly ILogger<AccountingConsumer> _logger;
    #if UseOpenSearch 
    private readonly IElasticRepository<AccountMessage> _elasticRepository;
    #endif

    public AccountingConsumer(
        ILogger<AccountingConsumer> logger
        #if UseOpenSearch
        , IElasticRepository<AccountMessage> elasticRepository
        #endif
        )
    {
        _logger = logger;
        #if UseOpenSearch
        _elasticRepository = elasticRepository;
        #endif
    }

    [Consume(Type = typeof(KafkaExtraConfig), Property = nameof(KafkaExtraConfig.KafkaTopic2))]
    private async Task HandleKafkaMessages(List<AccountMessage> messages)
    {
        _logger.LogInformation("Kafka messages received...");
        foreach (var message in messages)
        {
            #if UseOpenSearch
            try
            {
                await _elasticRepository.Add(message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to index message: {message} and exception: {exception} at {timestamp}",
                    JsonConvert.SerializeObject(message), ex.Message, DateTime.UtcNow);
            }
            #else
            await Task.Delay(0);
            //do something
            #endif
        }
    }
}