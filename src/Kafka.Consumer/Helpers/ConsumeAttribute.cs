namespace Kafka.Consumer.Helpers;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ConsumeAttribute : Attribute
{
    public Type Type { get; set; } = null!;
    public string Topics { get; set; } = null!;
}
