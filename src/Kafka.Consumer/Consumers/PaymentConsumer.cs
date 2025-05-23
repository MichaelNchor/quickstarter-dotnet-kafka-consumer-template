namespace Kafka.Consumer.Consumers;

public class PaymentConsumer : KafkaConsumerBase
{
    private readonly ILogger<PaymentConsumer> _logger;
    private readonly IElasticRepository<PaymentMessage> _elasticRepository;

    public PaymentConsumer(ILogger<PaymentConsumer> logger, IElasticRepository<PaymentMessage> elasticRepository)
    {
        _logger = logger;
        _elasticRepository = elasticRepository;
    }

    [Consume(Type = typeof(KafkaConsumerConfig), Property = nameof(KafkaConsumerConfig.TopicsAsSingleString))]
    private async Task HandleKafkaMessages(List<PaymentMessage> messages)
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
                _logger.LogError("Failed to index message: {message} and exception: {exception} at {timestamp}",
                    JsonConvert.SerializeObject(message), ex.Message, DateTime.UtcNow);
            }
        }
    }
}