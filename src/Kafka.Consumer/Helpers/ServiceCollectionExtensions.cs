using Kafka.Consumer.Services.Provider;
using Kafka.Consumer.Workers;

namespace Kafka.Consumer.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KafkaConfig>()
            .Bind(configuration.GetSection(nameof(KafkaConfig)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddHostedService<BackgroundRunner>()
            .AddSingleton<KafkaConsumer>()
            .AddSingleton<IKafkaConsumerService, KafkaConsumerService<KafkaMessage>>();
        return services;
    }
    
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(nameof(OpenSearchConfig));
        var openSearchConfig = section.Get<OpenSearchConfig>();
        if (openSearchConfig is null)
            throw new InvalidOperationException("Failed to bind OpenSearch configuration. Check 'OpenSearchConfig' section in appsettings.");
        var settings = new ConnectionSettings(new Uri(openSearchConfig.Uri))
            .DefaultIndex(openSearchConfig.IndexName);
        return services
            .Configure<OpenSearchConfig>(section)
            .AddSingleton<IElasticClient>(new ElasticClient(settings))
            .AddTransient(typeof(IElasticRepository<>), typeof(ElasticRepository<>));
    }
}