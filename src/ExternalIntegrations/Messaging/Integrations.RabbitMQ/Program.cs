using Integrations.RabbitMQ;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton(_ => new RabbitConnection("localhost"));
        
        services.AddHostedService<Worker>();
        services.AddHostedService<Consumer>();
    })
    .Build();

await host.RunAsync();