namespace Kafka.Consumer.Workers;

public class BackgroundRunner : BackgroundService
{
    private readonly ILogger<BackgroundRunner> _logger;
    private readonly IServiceProvider _serviceProvider;
    private List<IKafkaConsumerLogic> _activeConsumers = new();

    public BackgroundRunner(ILogger<BackgroundRunner> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        _logger.LogInformation("BackgroundRunner starting at {timestamp}", DateTime.UtcNow);
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            _activeConsumers = scope.ServiceProvider.GetServices<IKafkaConsumerLogic>().ToList();
            await Parallel.ForEachAsync(_activeConsumers, ctx, async (consumer, token) =>
            {
                try
                {
                    _logger.LogInformation("Starting Kafka consumer: {consumer} at {timestamp}", consumer.GetType().Name, DateTime.UtcNow);
                    await consumer.StartConsumingAsync(token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    _logger.LogInformation("Kafka consumer {consumer} canceled at {timestamp}.", consumer.GetType().Name, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error: {error} in Kafka consumer {consumer} at {timestamp}", ex.Message, consumer.GetType().Name, DateTime.UtcNow);
                }
            });
        }
        catch (OperationCanceledException) when (ctx.IsCancellationRequested)
        {
            _logger.LogInformation("BackgroundRunner cancelled at {timestamp}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {error} in BackgroundRunner at {timestamp}", ex.Message, DateTime.UtcNow);
        }
    }

    public override async Task StopAsync(CancellationToken ctx)
    {
        _logger.LogInformation("BackgroundRunner stopping at {timestamp}", DateTime.UtcNow);
        try
        {
            _activeConsumers.ForEach(consumer => consumer.Dispose());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {error} in BackgroundRunner at {timestamp}", ex.Message, DateTime.UtcNow);
        }
        await base.StopAsync(ctx);
    }
}