namespace Shared.Messaging;

public interface IMessageProducer
{
    Task<bool> SendAsync(Message message);
}