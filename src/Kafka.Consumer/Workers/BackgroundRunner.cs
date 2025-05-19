namespace Kafka.Consumer.Workers;

public class BackgroundRunner(ILogger<BackgroundRunner> logger, IKafkaConsumerService kafkaConsumerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        logger.LogInformation("Service started at {timestamp}", DateTime.UtcNow);
        
        // Consume the Kafka consumer
        await kafkaConsumerService.Consume(ctx);
    }

    public override Task StopAsync(CancellationToken ctx)
    {
        logger.LogInformation("Service stopping at {timestamp}", DateTime.UtcNow);
        return base.StopAsync(ctx);
    }
}