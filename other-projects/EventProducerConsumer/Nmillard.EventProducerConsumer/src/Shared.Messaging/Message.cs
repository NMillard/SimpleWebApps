namespace Shared.Messaging;

public class Message
{
    public string ExchangeName { get; set; }
    public string Topic { get; set; }
    public object Payload { get; set; }
}