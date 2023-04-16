using System.Reflection;
using Microsoft.Extensions.Azure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultBufferSize = 100;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddAzureClients(factoryBuilder =>
{
    string? connectionString = builder.Configuration.GetConnectionString("storage");
    if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Missing blob storage connection string");
    factoryBuilder.AddBlobServiceClient(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();