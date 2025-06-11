namespace __PROJECT_NAME__.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KafkaConsumerConfig>()
            .Bind(configuration.GetSection(nameof(KafkaConsumerConfig)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddOptions<KafkaExtraConfig>()
            .Bind(configuration.GetSection(nameof(KafkaExtraConfig)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services
            .AddHostedService<BackgroundRunner>()
            // register consumer and consumer message
            .AddSingleton<IKafkaConsumerLogic, KafkaConsumerLogic<AccountMessage, AccountingConsumer>>()
            .AddSingleton<IKafkaConsumerLogic, KafkaConsumerLogic<PaymentMessage, PaymentConsumer>>();
        return services;
    }
    
    #if UseOpenSearch
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(nameof(OpenSearchConfig));
        var openSearchConfig = section.Get<OpenSearchConfig>();
        if (openSearchConfig is null)
            ArgumentNullException.ThrowIfNull(openSearchConfig, nameof(openSearchConfig));
        var settings = new ConnectionSettings(new Uri(openSearchConfig.Uri))
            .DefaultIndex(openSearchConfig.IndexName);
        return services
            .Configure<OpenSearchConfig>(section)
            .AddSingleton<IElasticClient>(new ElasticClient(settings))
            .AddTransient(typeof(IElasticRepository<>), typeof(ElasticRepository<>));
    }
    #endif
}