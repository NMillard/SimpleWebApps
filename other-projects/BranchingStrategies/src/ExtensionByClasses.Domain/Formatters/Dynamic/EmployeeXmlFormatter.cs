using System.Xml;
using System.Xml.Serialization;

namespace ExtensionByClasses.Domain.Formatters.Dynamic;

public class EmployeeXmlFormatter : IEmployeeFormatter
{
    private readonly XmlWriterSettings xmlSettings = new()
    {
        Indent = true,
        NewLineChars = Environment.NewLine,
        NewLineHandling = NewLineHandling.Replace,
        OmitXmlDeclaration = false
    };
    
    public string Format(Employee employee)
    {
        EmployeeXml employeeXml = EmployeeXml.FromEmployee(employee);
        var xmlSerializer = new XmlSerializer(employeeXml.GetType());
            
        using var writer = new StringWriter();
        using var xmlWriter = XmlWriter.Create(writer, xmlSettings);
        xmlSerializer.Serialize(xmlWriter, employeeXml);

        return writer.ToString();
    }

    public string FormattingType => "Xml";
    
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