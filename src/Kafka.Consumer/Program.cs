namespace Kafka.Consumer;

class Program
{
    public static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging
                    .ClearProviders()
                    .AddConsole()
                    .AddDebug()
                    .AddConfiguration(context.Configuration.GetSection("Logging"));
            })
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