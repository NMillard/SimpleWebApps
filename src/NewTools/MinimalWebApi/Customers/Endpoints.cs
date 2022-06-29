using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MinimalWebApi.Customers; 

public static class CustomerEndpoints {
    public static void MapCustomerEndpoints(this WebApplication app) {
        app.MapGet("/", () => Results.Ok("Hello")).WithName("root");
        app.MapGet("/api/customers", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetCustomerQuery())));
        app.MapPost("/api/customers/RegisterNew", async (CustomerRequest customer, IMediator mediator) => {
            if (string.IsNullOrEmpty(customer.Name)) return Results.BadRequest();

            bool result = await mediator.Send(new CreateCustomerCommand(new Customer(customer.Name)));
            return Results.Ok(result);
        });
    }
}