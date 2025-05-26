namespace Kafka.Consumer.Consumers;

public class PaymentConsumer : KafkaConsumerBase
{
    private readonly ILogger<PaymentConsumer> _logger;
    #if UseOpenSearch 
    private readonly IElasticRepository<AccountMessage> _elasticRepository;
    #endif
    
    public PaymentConsumer(ILogger<PaymentConsumer> logger
        #if UseOpenSearch
        , IElasticRepository<PaymentMessage> elasticRepository
        #endif
        )
    {
        _logger = logger;
        #if UseOpenSearch
        _elasticRepository = elasticRepository;
        #endif
    }

    [Consume(Type = typeof(KafkaConsumerConfig), Property = nameof(KafkaConsumerConfig.TopicsAsSingleString))]
    private async Task HandleKafkaMessages(List<PaymentMessage> messages)
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