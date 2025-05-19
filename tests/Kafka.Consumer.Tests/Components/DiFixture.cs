namespace Kafka.Consumer.Tests.Components;

public class DiFixture
{
    public IServiceProvider ServiceProvider { get; }
    public DiFixture()
    {
        var services = new ServiceCollection();
        services.AddSingleton((sp) => ConfigurationManager.GetConfiguration());
        ServiceProvider = services.BuildServiceProvider();
    }
}