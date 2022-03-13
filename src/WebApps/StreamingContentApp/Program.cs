using Microsoft.AspNetCore.Builder;
using StreamingContentApp.Abstractions;
using StreamingContentApp.RealEstateFeature;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRealEstateRepository, InMemoryRealEstateRepository>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();