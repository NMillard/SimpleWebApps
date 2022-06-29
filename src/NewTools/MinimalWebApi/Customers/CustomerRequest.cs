namespace MinimalWebApi.Customers; 

public record CustomerRequest {
    public string? Name { get; init; }
}