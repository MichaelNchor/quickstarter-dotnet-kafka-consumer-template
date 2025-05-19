namespace Kafka.Consumer.Tests.Components;

public class ConfigurationManager
{
    private static IConfiguration _configuration = null!;

    public static IConfiguration GetConfiguration()
    {
        ArgumentNullException.ThrowIfNull(_configuration);
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true);

        _configuration = builder.Build();

        return _configuration;
    }
}