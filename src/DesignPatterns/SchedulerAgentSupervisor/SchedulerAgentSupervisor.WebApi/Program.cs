using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchedulerAgentSupervisor.WebApi.BackgroundServices;
using SchedulerAgentSupervisor.WebApi.Converters;
using SchedulerAgentSupervisor.WebApi.Domain;
using SchedulerAgentSupervisor.WebApi.Persistence;
using DateOnlyConverter = SchedulerAgentSupervisor.WebApi.Converters.DateOnlyConverter;
using TimeOnlyConverter = SchedulerAgentSupervisor.WebApi.Converters.TimeOnlyConverter;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;

// Add services to the container.

IServiceCollection services = builder.Services;
services.AddControllers().AddJsonOptions(o => {
    o.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
    o.JsonSerializerOptions.Converters.Add(new NullableDateOnlyConverter());
    o.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAzureClients(builder => builder.AddQueueServiceClient(config.GetConnectionString("storageAccount")));

services
    .AddScoped<IScheduledTaskDefinition, SayHelloTask>()
    .AddScoped<IScheduledTaskDefinition, QueueTask>()
    .AddHostedService<ScheduleCheckerBackgroundService>();

services.AddDbContext<AppDbContext>(o => o.UseSqlServer(config.GetConnectionString("sql")));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();