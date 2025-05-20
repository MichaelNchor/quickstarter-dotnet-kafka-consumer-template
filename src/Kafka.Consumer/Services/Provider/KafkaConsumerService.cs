using System.Linq.Expressions;

namespace Kafka.Consumer.Services.Provider;

public class KafkaConsumerService<T> : IKafkaConsumerService, IDisposable where T : class
{
    private readonly ILogger<KafkaConsumerService<T>> _logger;
    private readonly IConsumer<Null, string> _consumer;
    private readonly KafkaConsumerConfig _kafkaConsumerConfig;
    private readonly List<ConsumeResult<Null, string>> _messageBuffer = [];
    private DateTime _lastFlushTime = DateTime.UtcNow;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Func<KafkaConsumer, List<T>, Task>> _cacheConsumeHandlers;
    public KafkaConsumerService(
        ILogger<KafkaConsumerService<T>> logger,
        IOptions<KafkaConfig> kafkaConfig,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        var kafkaConsumerConfig = kafkaConfig.Value.Consumer;
        var config = new ConsumerConfig 
        {
            BootstrapServers = kafkaConsumerConfig.BootstrapServers,
            GroupId = kafkaConsumerConfig.GroupId,
            ClientId = kafkaConsumerConfig.ClientId,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaConsumerConfig.SecurityProtocol, ignoreCase: true),
            AutoOffsetReset = AutoOffsetReset.Earliest 
        };
        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _consumer.Subscribe(kafkaConsumerConfig.Topics);
        _kafkaConsumerConfig = kafkaConfig.Value.Consumer;
        _serviceProvider = serviceProvider;
        _cacheConsumeHandlers = CacheConsumeHandlers();
    }
    
    public async Task StartConsumingAsync(CancellationToken ctx)
    {
        _logger.LogInformation("Consumer started at {timestamp}", DateTime.UtcNow);
        while (!ctx.IsCancellationRequested)
        {
            try
            {
                //consume from kafka
                ConsumeResult<Null, string> consumeResult = _consumer.Consume(ctx);
                if (consumeResult?.Message?.Value == null) continue;
                
                //add consume result to buffer
                _messageBuffer.Add(consumeResult);

                // Flush the buffer if it reaches a certain size or after a certain time
                TimeSpan timeSinceLastFlush = DateTime.UtcNow - _lastFlushTime;
                if (_messageBuffer.Count >= _kafkaConsumerConfig.BatchSize ||
                    timeSinceLastFlush > TimeSpan.FromSeconds(_kafkaConsumerConfig.BatchIntervalInSeconds))
                {
                    // Process the batch
                    await ProcessBatchAsync(consumeResult);
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError("Error consuming message: {Error} @ {timestamp}",
                    e.Error.Reason, DateTime.UtcNow);
                break;
            }
            await Task.Yield();
        }
        _consumer.Close();
        _logger.LogInformation("Consumer stopped @ {timestamp}", DateTime.UtcNow);
    }

    private async Task ProcessBatchAsync(ConsumeResult<Null, string> consumeResult)
    {
        // Initialize the KafkaConsumer and Context
        IServiceScope scope = _serviceProvider.CreateScope();
        KafkaConsumer consumer = scope.ServiceProvider.GetRequiredService<KafkaConsumer>();
        consumer.Context = new KafkaContext(_messageBuffer, _kafkaConsumerConfig.BootstrapServers);
                    
        // deserialize the messages
        List<T>? deserializedMessages = _messageBuffer.ConvertBufferListToType<T>();
        if (deserializedMessages == null)
        {
            _logger.LogError("Deserializing messages: {messages} failed @ {timestamp}", 
                JsonConvert.SerializeObject(_messageBuffer), DateTime.UtcNow);
            return;
        }
                    
        // Call each method with the buffer
        foreach (var method in _cacheConsumeHandlers)
        {
            // Invoke the method with the deserialized messages
            await method(consumer, deserializedMessages);
            _lastFlushTime = DateTime.UtcNow; 
            _logger.LogInformation("Consumed Message: {message} to topic: {topic} @ {timestamp}",
                JsonConvert.SerializeObject(consumeResult.Message), consumeResult.Topic, DateTime.UtcNow);
        }
        _messageBuffer.Clear();
    }
    
    private static List<Func<KafkaConsumer, List<T>, Task>> CacheConsumeHandlers()
    {
        var handlers = new List<Func<KafkaConsumer, List<T>, Task>>();
        List<MethodInfo> consumeMethodInfos = typeof(KafkaConsumer)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<ConsumeAttribute>() != null 
                        && m.GetParameters().Length == 1 
                        && m.GetParameters()[0].ParameterType == typeof(List<T>))
            .ToList();
        
        foreach (var methodInfo in consumeMethodInfos)
        {
            ParameterExpression consumerParameter = Expression.Parameter(typeof(KafkaConsumer), "consumer");
            ParameterExpression messagesParameter = Expression.Parameter(typeof(List<T>), "messages");
            MethodCallExpression methodCall = Expression.Call(consumerParameter, methodInfo, messagesParameter);
            Expression<Func<KafkaConsumer,List<T>,Task>> lambdaExpression = Expression.Lambda<Func<KafkaConsumer, List<T>, Task>>(
                methodCall, consumerParameter, messagesParameter);
            handlers.Add(lambdaExpression.Compile());
        }
        
        return handlers;
    }
    
    public void Dispose()
    {
        _logger.LogInformation("Disposing KafkaConsumerService<{type}> synchronously", typeof(T).Name);
        _consumer.Dispose();
        GC.SuppressFinalize(this);
    }
}