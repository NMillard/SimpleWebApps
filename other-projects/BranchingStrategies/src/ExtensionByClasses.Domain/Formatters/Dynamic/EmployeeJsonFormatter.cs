using System.Text.Json;

namespace ExtensionByClasses.Domain.Formatters.Dynamic;

public class EmployeeJsonFormatter : IEmployeeFormatter
{
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    public string Format(Employee employee)
    {
        return JsonSerializer.Serialize(employee, jsonOptions);
    }

    public string FormattingType => "Json";
}