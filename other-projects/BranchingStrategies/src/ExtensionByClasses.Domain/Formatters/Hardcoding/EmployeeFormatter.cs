using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

namespace ExtensionByClasses.Domain.Formatters.Hardcoding;

public class EmployeeFormatter(IOptionsMonitor<AvaiableFormatOptions> options)
{
    public IReadOnlyList<string> GetAvailableFormats()
    {
        return Enum.GetNames(typeof(OutputFormat))
            .Where(of => options.CurrentValue.EmployeeFormats.Contains(of))
            .ToList();
    }

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    private readonly XmlWriterSettings xmlSettings = new()
    {
        Indent = true,
        NewLineChars = Environment.NewLine,
        NewLineHandling = NewLineHandling.Replace,
        OmitXmlDeclaration = false
    };

    /// <summary>
    /// Using a traditional branching technique doesn't scale well in terms of collaborative scalability,
    /// readability, or maintainability.
    ///
    /// Imagine having a bug in both the PDF and XML output. The bugs are added to a kanban/scrum board as separate
    /// tickets, and two developers jump on them and resolve the bug in two different PRs. You can easily see how
    /// this would result in merge conflicts.
    /// </summary>
    public string Format(Employee employee, OutputFormat outputFormat)
    {
        bool unknownFormat = !GetAvailableFormats().Contains(outputFormat.ToString());
        if (unknownFormat) return "";
        
        if (outputFormat == OutputFormat.Json)
        {
            return JsonSerializer.Serialize(employee, jsonOptions);
        }
        else if (outputFormat == OutputFormat.Xml)
        {
            EmployeeXml employeeXml = EmployeeXml.FromEmployee(employee);
            var xmlSerializer = new XmlSerializer(employeeXml.GetType());
            
            using var writer = new StringWriter();
            using var xmlWriter = XmlWriter.Create(writer, xmlSettings);
            xmlSerializer.Serialize(xmlWriter, employeeXml);

            return writer.ToString();
        } else if (outputFormat == OutputFormat.Pdf)
        {
            var html = $"""
                        <h1>{employee.Name}</h1>
                        <p>Id: {employee.Id}</p> 
                        <p>Hiring date: {employee.HiringDate.ToString("yyyy MMMM dd")}</p> 
                        """;

            var browserFetcher = new BrowserFetcher();
            browserFetcher.DownloadAsync().Wait();
            using IBrowser browser = Puppeteer.LaunchAsync(new LaunchOptions {Headless = true}).Result;
            using IPage page = browser.NewPageAsync().Result;
        
            page.SetContentAsync(html).Wait();
            byte[] bytes = page.PdfDataAsync().Result;
        
            return Convert.ToBase64String(bytes);
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

public enum OutputFormat
{
    Json,
    Xml,
    Pdf // new
}