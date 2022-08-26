using Shared.Messaging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMessaging("localhost");

WebApplication app = builder.Build();

app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();