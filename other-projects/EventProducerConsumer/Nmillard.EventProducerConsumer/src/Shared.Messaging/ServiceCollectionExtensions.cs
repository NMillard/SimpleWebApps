using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, string hostName)
    {
        services.AddSingleton(_ => new RabbitMqConnection(hostName));
        services.AddScoped<IMessageProducer, RabbitMqMessageProducer>();
        
        return services;
    }

    public static IServiceCollection AddMessageConsumer<THandler, TMessage>(
        this IServiceCollection services,
        string hostName)
        where THandler : MessageHandler<TMessage>
    {
        string? clientName = Assembly.GetCallingAssembly().GetName().Name;
        
        services.AddSingleton(_ => new RabbitMqConnection(hostName, clientName));
        services.AddHostedService<MessageConsumer<THandler, TMessage>>();
        
        return services;
    }
}