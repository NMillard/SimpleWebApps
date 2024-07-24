namespace ExtensionByClasses.Domain.Formatters.Dynamic;

public interface IEmployeeFormatter
{
    string Format(Employee employee);
    string FormattingType { get; }
}