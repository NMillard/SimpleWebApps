using Consumer.WebApi;
using Shared.Messaging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DefaultMessageHandler>();
builder.Services.AddMessageConsumer<DefaultMessageHandler, DefaultMessage>("localhost");

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.Run();