using Kafka.Consumer.Extensions;

namespace Kafka.Consumer;

class Program
{
    public static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services
                    .AddKafkaConsumer(context.Configuration)
                    .AddOpenSearch(context.Configuration);
            })
            .Build();
        
        await host.RunAsync();
    }
}