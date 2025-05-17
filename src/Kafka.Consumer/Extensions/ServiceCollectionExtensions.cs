using Kafka.Consumer.Services;
using Nest;

namespace Kafka.Consumer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KafkaConfig>()
            .Bind(configuration.GetSection(nameof(KafkaConfig)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHostedService<KafkaConsumerService>();
        return services;
    }
    
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(nameof(OpenSearchConfig));
        var openSearchConfig = section.Get<OpenSearchConfig>();
        if (openSearchConfig is null)
            throw new InvalidOperationException("Failed to bind OpenSearch configuration. Check 'OpenSearchConfig' section in appsettings.");
        services.AddSingleton<IElasticClient, ElasticClient>();
        var settings = new ConnectionSettings(new Uri(openSearchConfig.Uri))
            .DefaultIndex(openSearchConfig.IndexName);
        var elasticClient = new ElasticClient(settings);
        return services
            .Configure<OpenSearchConfig>(section)
            .AddSingleton<IElasticClient>(elasticClient)
            .AddSingleton<IElasticClientService, ElasticClientService>();
    }
}