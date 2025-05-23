namespace Kafka.Consumer.Workers;

public class BackgroundRunner : BackgroundService
{
    private readonly ILogger<BackgroundRunner> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BackgroundRunner(ILogger<BackgroundRunner> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        _logger.LogInformation("BackgroundRunner starting at {Timestamp}", DateTime.UtcNow);

        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IEnumerable<IKafkaConsumerLogic> consumerLogics = scope.ServiceProvider.GetServices<IKafkaConsumerLogic>();
            await Parallel.ForEachAsync(consumerLogics, ctx, async (consumer, token) =>
            {
                await consumer.StartConsumingAsync(token);
            });
        }
        catch (OperationCanceledException) when (ctx.IsCancellationRequested)
        {
            _logger.LogInformation("BackgroundRunner cancelled at {Timestamp}", DateTime.UtcNow);
        }
    }

    public override async Task StopAsync(CancellationToken ctx)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IEnumerable<IKafkaConsumerLogic> consumerLogics = scope.ServiceProvider.GetServices<IKafkaConsumerLogic>();
        Parallel.ForEach(consumerLogics, consumer =>
        {
            consumer.Dispose();
        });
        await base.StopAsync(ctx);
        _logger.LogInformation("BackgroundRunner stopping at {Timestamp}", DateTime.UtcNow);
    }
}