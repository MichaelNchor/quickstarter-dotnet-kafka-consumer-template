using System.Linq.Expressions;
using Kafka.Consumer.Attributes;
using Kafka.Consumer.Extensions;

namespace Kafka.Consumer.Services;

public class KafkaConsumerLogic<TMessage, TConsumer> : IKafkaConsumerLogic, IDisposable where TMessage : class where TConsumer : KafkaConsumerBase
{
    private readonly ILogger<KafkaConsumerLogic<TMessage, TConsumer>> _logger;
    private readonly IConsumer<Null, string> _consumer;
    private readonly KafkaConsumerConfig _kafkaConsumerConfig;
    private readonly List<ConsumeResult<Null, string>> _messageBuffer = new();
    private DateTime _lastFlushTime = DateTime.UtcNow;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Func<TConsumer, List<TMessage>, Task>> _cacheConsumeHandlers;
    public KafkaConsumerLogic(
        ILogger<KafkaConsumerLogic<TMessage,TConsumer>> logger,
        IOptions<KafkaConsumerConfig> kafkaConsumerConfig,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        var consumerConfig = kafkaConsumerConfig.Value;
        var config = new ConsumerConfig 
        {
            BootstrapServers = consumerConfig.BootstrapServers,
            GroupId = consumerConfig.GroupId,
            ClientId = consumerConfig.ClientId,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(consumerConfig.SecurityProtocol, ignoreCase: true),
            AutoOffsetReset = AutoOffsetReset.Earliest 
        };
        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _kafkaConsumerConfig = kafkaConsumerConfig.Value;
        _serviceProvider = serviceProvider;
        _cacheConsumeHandlers = CacheConsumeHandlers();
        _consumer.Subscribe(GetAttributeTopics());
    }
    
    public async Task StartConsumingAsync(CancellationToken ctx)
    {
        _logger.LogInformation("Consumers started at {timestamp}", DateTime.UtcNow);
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
                // break;
            }
            await Task.Yield();
        }
        _consumer.Close();
        _logger.LogInformation("Consumers stopped @ {timestamp}", DateTime.UtcNow);
    }

    private async Task ProcessBatchAsync(ConsumeResult<Null, string> consumeResult)
    {
        // Initialize the TConsumer and Context
        using IServiceScope scope = _serviceProvider.CreateScope();
        TConsumer consumer = ActivatorUtilities.CreateInstance<TConsumer>(scope.ServiceProvider);
        consumer.Context = new KafkaContext(_messageBuffer, _kafkaConsumerConfig.BootstrapServers);
                    
        // deserialize the messages
        List<TMessage>? deserializedMessages = _messageBuffer.ConvertBufferListToType<TMessage>();
        if (deserializedMessages == null || deserializedMessages.Count == 0)
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
    
    private static List<Func<TConsumer, List<TMessage>, Task>> CacheConsumeHandlers()
    {
        var handlers = new List<Func<TConsumer, List<TMessage>, Task>>();
        List<MethodInfo> consumeMethodInfos = typeof(TConsumer)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<ConsumeAttribute>() != null 
                        && m.GetParameters().Length == 1 
                        && m.GetParameters()[0].ParameterType == typeof(List<TMessage>))
            .ToList();
        
        foreach (var methodInfo in consumeMethodInfos)
        {
            ParameterExpression consumerParameter = Expression.Parameter(typeof(TConsumer), "consumer");
            ParameterExpression messagesParameter = Expression.Parameter(typeof(List<TMessage>), "messages");
            MethodCallExpression methodCall = Expression.Call(consumerParameter, methodInfo, messagesParameter);
            Expression<Func<TConsumer,List<TMessage>,Task>> lambdaExpression = Expression.Lambda<Func<TConsumer, List<TMessage>, Task>>(
                methodCall, consumerParameter, messagesParameter);
            handlers.Add(lambdaExpression.Compile());
        }
        
        return handlers;
    }
    
    private List<string> GetAttributeTopics()
    {
        var topicList = new List<string>();
        
        var topicByProperty = typeof(TConsumer)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => m.GetCustomAttribute<ConsumeAttribute>())
            .Where(attr => attr != null && !string.IsNullOrWhiteSpace(attr.Property))
            .ToList();

        foreach (var attr in topicByProperty)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            // Construct IOptions<Type> or similar
            Type optionsType = typeof(IOptions<>).MakeGenericType(attr?.Type!);
            var optionsObj = scope.ServiceProvider.GetRequiredService(optionsType);

            // Get the "Value" property from IOptions<Type>
            object? configInstance = optionsType
                .GetProperty("Value")?
                .GetValue(optionsObj);

            // Get the Kafka topic property
            string? topic = attr?.Type
                .GetProperty(attr.Property)?
                .GetValue(configInstance) as string;
            
            topicList.Add(topic!);
        }
        
        var topicByTopic = typeof(TConsumer)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => m.GetCustomAttribute<ConsumeAttribute>())
            .Where(attr => attr != null && !string.IsNullOrWhiteSpace(attr.Topic))
            .Select(attr => attr?.Topic!)
            .Distinct()
            .ToList();
        
        topicList.AddRange(topicByTopic);
        
        return topicList;
    }
    
    public void Dispose()
    {
        _logger.LogInformation("Disposing KafkaConsumerLogic<{type}> synchronously", typeof(TMessage).Name);
        _consumer.Dispose();
        GC.SuppressFinalize(this);
    }
}