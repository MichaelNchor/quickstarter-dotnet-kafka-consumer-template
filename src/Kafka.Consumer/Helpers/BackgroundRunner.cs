namespace Kafka.Consumer.Helpers;

public class BackgroundRunner(ILogger<BackgroundRunner> logger, IKafkaConsumerLogic kafkaConsumerLogic) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        logger.LogInformation("Service started at {timestamp}", DateTime.UtcNow);
        
        // Start the Kafka consumer
        await kafkaConsumerLogic.StartConsumer(ctx);
    }

    public override Task StopAsync(CancellationToken ctx)
    {
        logger.LogInformation("Service stopping at {timestamp}", DateTime.UtcNow);
        return base.StopAsync(ctx);
    }
}