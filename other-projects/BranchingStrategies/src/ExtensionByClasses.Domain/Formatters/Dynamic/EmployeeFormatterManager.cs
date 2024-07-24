using Microsoft.Extensions.Logging;

namespace ExtensionByClasses.Domain.Formatters.Dynamic;

public class EmployeeFormatterManager(ILogger<EmployeeFormatterManager> logger)
{
    private readonly Dictionary<string, IEmployeeFormatter> formatters = [];
    
    public IReadOnlyList<string> GetAvailableFormats() 
        => formatters.Select(kv => kv.Key).ToList();

    public string Format(Employee employee, string format)
    {
        bool hasFormatter = formatters.ContainsKey(format);
        return hasFormatter
            ? formatters[format].Format(employee)
            : throw new UnknownFormatException(format, GetAvailableFormats());
    }

    public EmployeeFormatterManager Add(IEmployeeFormatter formatter)
    {
        logger.LogEmployeeFormatterAdded(formatter.GetType(), formatter.FormattingType);
        
        formatters.Add(formatter.FormattingType, formatter);
        return this;
    }

    public EmployeeFormatterManager Add(IEnumerable<IEmployeeFormatter> employeeFormatters)
    {
        foreach (IEmployeeFormatter employeeFormatter in employeeFormatters) Add(employeeFormatter);
        return this;
    }
}

public class UnknownFormatException(string formatType, IEnumerable<string> availableFormats)
    : Exception($"Unknown type {formatType}. Available formats are: {string.Join(",", availableFormats)}");