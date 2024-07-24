using System.Xml.Serialization;

namespace ExtensionByClasses.Domain.Formatters.Hardcoding;

public class JsonFormatter
{
    public string Format(Employee employee)
    {
        throw new NotImplementedException();
    }
}

public class XmlFormatter
{
    public string Format(Employee employee)
    {
        throw new NotImplementedException();
    }
}

public class PdfFormatter
{
    public string Format(Employee employee)
    {
        throw new NotImplementedException();
    }
}

public class EmployeeFormatterRefactored(
    JsonFormatter json,
    XmlFormatter xml,
    PdfFormatter pdf)
{
    public IReadOnlyList<string> GetAvailableFormats()
    {
        return Enum.GetNames(typeof(OutputFormat))
            .ToList();
    }

    public string Format(Employee employee, OutputFormat outputFormat)
    {
        if (outputFormat == OutputFormat.Json)
        {
            return json.Format(employee);
        }
        else if (outputFormat == OutputFormat.Xml)
        {
            return xml.Format(employee);
        } 
        else if (outputFormat == OutputFormat.Pdf)
        {
            return pdf.Format(employee);
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// This type is only used in the format == FormatOutput.Xml branch.
    /// </summary>
    [XmlType(typeName: "Employee")]
    public record EmployeeXml
    {
        [XmlElement]
        public Guid Id { get; set; }
        
        [XmlElement]
        public string Name { get; set; }
        
        [XmlElement]
        public DateTime HiringDate { get; set; }
        
        public static EmployeeXml FromEmployee(Employee employee)
        {
            return new EmployeeXml
            {
                Id = employee.Id, 
                Name = employee.Name,
                HiringDate = new DateTime(employee.HiringDate, TimeOnly.MinValue)
            };
        }
    }
}
