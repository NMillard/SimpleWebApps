namespace MinimalWebApi.Customers; 

internal class CustomerRepository {
    private readonly AppDbContext context;

    public CustomerRepository(AppDbContext context) {
        this.context = context;
    }
    
    public Customer Get() => new Customer("Nicklas");

    public bool Create(Customer customer) => true;
}