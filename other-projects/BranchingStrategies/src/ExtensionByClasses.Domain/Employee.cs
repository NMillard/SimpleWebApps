namespace ExtensionByClasses.Domain;

public class Employee
{
    public Employee() : this(Guid.NewGuid()) { }
    public Employee(Guid id) => Id = id;

    public Guid Id { get; }
    public required string Name { get; init; }
    public required DateOnly HiringDate { get; init; }
}