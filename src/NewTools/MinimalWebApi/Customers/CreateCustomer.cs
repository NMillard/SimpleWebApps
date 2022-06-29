using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MinimalWebApi.Customers; 

public record CreateCustomerCommand(Customer Customer) : IRequest<bool>;

internal class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, bool> {
    private readonly CustomerRepository repository;

    public CreateCustomerHandler(CustomerRepository repository) {
        this.repository = repository;
    }
    
    public Task<bool> Handle(CreateCustomerCommand request, CancellationToken cancellationToken) {
        bool result = repository.Create(request.Customer);
        return Task.FromResult<bool>(result);
    }
}