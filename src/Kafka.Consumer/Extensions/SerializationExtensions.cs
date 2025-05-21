namespace Kafka.Consumer.Extensions;

public static class SerializationExtensions
{
    public static List<T>? ConvertBufferListToType<T>(this List<ConsumeResult<Null, string>> messages) where T : class
    {
        try
        {
            var results = messages.Select(message => message.Message.Value.Deserialize<T>())
                .OfType<T>()
                .ToList();
            return results;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static string Serialize<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    
    public static T? Deserialize<T>(this string json) where T : class
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }
        return JsonConvert.DeserializeObject<T>(json) ?? null;
    }
}