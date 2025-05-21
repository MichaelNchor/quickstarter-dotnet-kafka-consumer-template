namespace Kafka.Consumer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ConsumeAttribute : Attribute
{
    public Type Type { get; set; } = null!;
    public string Property { get; set; } = null!;
    public string Topic { get; set; } = null!;
}
