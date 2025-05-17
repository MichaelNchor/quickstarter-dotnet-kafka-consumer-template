using Kafka.Consumer.Models;

namespace Kafka.Consumer.Services;

public interface IElasticClientService
{
    Task IndexRecord<T>(T message, CancellationToken ctx) where T : KafkaMessage;
}