using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalWebApi;
using MinimalWebApi.Customers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("sql")));
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddMediatR(typeof(CustomerRepository).Assembly);

WebApplication app = builder.Build();
app.MapCustomerEndpoints();

app.Run();