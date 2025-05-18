

namespace Kafka.Consumer;

public class Kafka(ILogger<Kafka> logger, IElasticClientService elasticClientService) : KafkaBase
{
    [Consumer(Type = typeof(IOptions<KafkaConsumerConfig>), Topics = nameof(KafkaConsumerConfig.TopicsAsSingleString))]
    private async Task HandleKafkaMessages(List<KafkaMessage> messages)
    {
        logger.LogInformation("Received message => {message} @ {timestamp}",  JsonConvert.SerializeObject(messages), DateTime.UtcNow);
        
        foreach (var message in messages)
        {
            logger.LogInformation("Processed message: {message} @ {timestamp}", message, DateTime.UtcNow);
            try
            {
                await elasticClientService.Add(message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to index message: {message} and exception: {exception} at {now}", 
                    JsonConvert.SerializeObject(message), ex.Message, DateTime.UtcNow);
            }
        }
    }
}