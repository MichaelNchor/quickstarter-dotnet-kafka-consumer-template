namespace Kafka.Consumer.Services.Provider;

public class KafkaConsumerService<T> : IKafkaConsumerService where T : class
{
    private readonly ILogger<KafkaConsumerService<T>> _logger;
    private readonly IConsumer<Null, string> _consumer;
    private readonly KafkaConsumerConfig _kafkaConsumerConfig;
    private readonly List<ConsumeResult<Null, string>> _buffer = [];
    private DateTime _lastFlushTime = DateTime.UtcNow;
    private readonly IServiceProvider _serviceProvider;
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
    }
    
    public async Task Consume(CancellationToken ctx)
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
                _buffer.Add(consumeResult);
                
                // Flush the buffer if it reaches a certain size or after a certain time
                TimeSpan timeSinceLastFlush = DateTime.UtcNow - _lastFlushTime;
                if (_buffer.Count >= _kafkaConsumerConfig.BatchSize || 
                    timeSinceLastFlush > TimeSpan.FromSeconds(_kafkaConsumerConfig.BatchIntervalInSeconds))
                {
                    await ProcessBufferAndFlush(consumeResult);
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

    private async Task ProcessBufferAndFlush(ConsumeResult<Null, string> consumeResult)
    {
        // Initialize the KafkaConsumer and Context
        ILogger<KafkaConsumer> logger = _serviceProvider.GetRequiredService<ILogger<KafkaConsumer>>();
                    
        // Initialize the ElasticRepository
        IElasticRepository elasticRepository = _serviceProvider.GetRequiredService<IElasticRepository>();
        KafkaConsumer consumer = new KafkaConsumer(logger,elasticRepository);
        consumer.Context = new KafkaContext(_buffer, _kafkaConsumerConfig.BootstrapServers);
                    
        // deserialize the messages
        List<T>? deserializedMessages = _buffer.ConvertBufferListToType<T>();
        if (deserializedMessages == null)
        {
            _logger.LogError("Deserializing messages: {messages} failed @ {timestamp}", 
                JsonConvert.SerializeObject(_buffer), DateTime.UtcNow);
            return;
        }
        // Use reflection to find methods with the ConsumeAttribute
        var methods = typeof(KafkaConsumer)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<ConsumeAttribute>() != null);
                    
        // Call each method with the buffer
        foreach (var method in methods)
        {
            // Check if the method has a single parameter of type List<T>
            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(List<T>))
            {
                // Invoke the method with the deserialized messages
                var task = (Task)method.Invoke(consumer, [deserializedMessages ?? []])!;
                await task;
                _lastFlushTime = DateTime.UtcNow; 
                _logger.LogInformation("Consumed Message: {message} to topic: {topic} @ {timestamp}",
                    JsonConvert.SerializeObject(consumeResult.Message), consumeResult.Topic, DateTime.UtcNow);
            }
            else
            {
                _logger.LogWarning("Method {methodName} does not match the expected signature @ {timestamp}", 
                    method.Name, DateTime.UtcNow);
            }
        }
        _buffer.Clear();
    }
}