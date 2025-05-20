namespace Kafka.Consumer.Workers;

public class BackgroundRunner : BackgroundService
{
    private readonly ILogger<BackgroundRunner> _logger;
    private readonly IKafkaConsumerService _kafkaConsumerService;

    public BackgroundRunner(ILogger<BackgroundRunner> logger, IKafkaConsumerService kafkaConsumerService)
    {
        _logger = logger;
        _kafkaConsumerService = kafkaConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        _logger.LogInformation("BackgroundRunner starting at {Timestamp}", DateTime.UtcNow);

        try
        {
            await _kafkaConsumerService.StartConsumingAsync(ctx);
        }
        catch (OperationCanceledException) when (ctx.IsCancellationRequested)
        {
            _logger.LogInformation("BackgroundRunner cancelled at {Timestamp}", DateTime.UtcNow);
        }
    }

    public override Task StopAsync(CancellationToken ctx)
    {
        _kafkaConsumerService.Dispose();
        base.StopAsync(ctx);
        _logger.LogInformation("BackgroundRunner stopping at {Timestamp}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}