namespace Kafka.Consumer.Workers;

public class BackgroundRunner(ILogger<BackgroundRunner> logger, IKafkaConsumerService kafkaConsumerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        logger.LogInformation("Service started at {timestamp}", DateTime.UtcNow);
        
        // Start the KafkaConsumer consumer
        await kafkaConsumerService.StartConsumer(ctx);
    }

    public override Task StopAsync(CancellationToken ctx)
    {
        logger.LogInformation("Service stopping at {timestamp}", DateTime.UtcNow);
        return base.StopAsync(ctx);
    }
}