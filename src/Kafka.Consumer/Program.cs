namespace Kafka.Consumer;

class Program
{
    public static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddLogging();
                services.AddKafkaConsumer(context.Configuration);
                services.AddOpenSearch(context.Configuration);
            })
            .Build();
        
        await host.RunAsync();
    }
}