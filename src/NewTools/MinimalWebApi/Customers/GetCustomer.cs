using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MinimalWebApi.Customers;

public record GetCustomerQuery : IRequest<Customer>;

public class GetCustomerHandler : IRequestHandler<GetCustomerQuery, Customer> {
    public Task<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken) {
        return Task.FromResult(new Customer("Niiicklas"));
    }
}