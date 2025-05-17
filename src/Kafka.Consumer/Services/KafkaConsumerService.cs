using Confluent.Kafka;
using Kafka.Consumer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;

namespace Kafka.Consumer.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly IElasticClientService _elasticClientService;
    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger, 
        IOptions<KafkaConfig> kafkaConfig,
        IElasticClientService elasticClientService)
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
        _consumer = new ConsumerBuilder<string, string>(config)
            .Build();
        _consumer.Subscribe(kafkaConsumerConfig.Topics);
        _elasticClientService = elasticClientService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ctx)
    {
        _logger.LogInformation("Consumer started at {timestamp}", DateTime.UtcNow);
        while (!ctx.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(ctx);
                var res = new KafkaMessage(consumeResult.Message.Value);
                await _elasticClientService.IndexRecord(res, ctx);
                _logger.LogInformation("Consumed Message: {message} to topic: {topic} @{timestamp}",
                    JsonConvert.SerializeObject(consumeResult.Message), 
                    consumeResult.Topic, DateTime.UtcNow);
            }
            catch (ConsumeException e)
            {
                _logger.LogError("Error consuming message: {Error} at {timestamp}", 
                    e.Error.Reason, DateTime.UtcNow);
                break;
            }
            await Task.Yield();
        }
        _consumer.Close();
        _logger.LogInformation("Consumer stopped at {timestamp}", DateTime.UtcNow);
    }
    
    public override void Dispose()
    {
        _consumer.Dispose();
    }
}