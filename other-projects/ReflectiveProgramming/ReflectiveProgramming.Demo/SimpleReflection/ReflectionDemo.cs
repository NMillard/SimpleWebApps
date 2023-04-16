using System.Reflection;
using System.Text;

namespace ReflectiveProgramming.Demo;

public class ReflectionDemo
{
    [Fact]
    public void Test1()
    {
        List<PropertyInfo> props = typeof(Employee)
            .GetProperties()
            .ToList();
    }

    [Fact]
    public void Test2()
    {
        var employees = new List<Employee>
        {
            new Employee
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = 1234,
                ManagerId = Guid.NewGuid(),
                Name = "Faxe Kondi",
                BirthDate = new DateOnly(1991, 11, 26),
                HiringDate = new DateOnly(2021, 12, 1)
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = 4321,
                ManagerId = Guid.NewGuid(),
                Name = "Håkon Æbletoft",
                BirthDate = new DateOnly(1981, 1, 15),
                HiringDate = new DateOnly(2003, 5, 1)
            }
        };

        var producer = new CsvProducer();
        string result = producer.FromRecords(employees);
    }
    
    [Fact]
    public void Test3()
    {
        var instance = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = 1234,
            ManagerId = Guid.NewGuid(),
            Name = "Faxe Kondi",
            BirthDate = new DateOnly(1991, 11, 26),
            HiringDate = new DateOnly(2021, 12, 1)
        };

        PropertyInfo propertyInfo = instance.GetType()
            .GetProperties()
            .Single(property => property.Name.Equals("Name"));

        object? value = propertyInfo.GetValue(instance); // value = "Faxe Kondi"
    }
}

public record Employee
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required int EmployeeNumber { get; init; }

    public required DateOnly BirthDate { get; init; }

    public required DateOnly HiringDate { get; init; }

    public required Guid ManagerId { get; init; }
}

public class CsvProducer
{
    public string FromRecords<TRecord>(IEnumerable<TRecord> records)
    {
        List<PropertyInfo> typeProperties = typeof(TRecord).GetProperties()
            .ToList();
        
        var stringBuilder = new StringBuilder();

        PropertyInfo last = typeProperties.Last();
        Predicate<PropertyInfo> notLast = p => !p.Equals(last);
        foreach (PropertyInfo property in typeProperties)
        {
            stringBuilder.Append(property.Name);
            if (notLast(property)) stringBuilder.Append(';');
        }

        List<TRecord> list = records.ToList();
        if (!list.Any()) return stringBuilder.ToString();

        stringBuilder.AppendLine();
        
        foreach (TRecord record in list)
        {
            IEnumerable<string> e = typeProperties
                .Select(property => property.GetValue(record)?.ToString()?? string.Empty);
            
            stringBuilder.AppendJoin(';', e).AppendLine();
        }

        return stringBuilder.ToString();
    }
}