using ExtensionByClasses.Domain.Formatters.Dynamic;
using FluentAssertions;

namespace ExtensionByClasses.Domain.Tests.Formatters.Dynamic;

public class EmployeeXmlFormatterShould
{
    [Fact]
    public void FormatEmployeeAsXml()
    {
        var employee = new Employee(Guid.Parse("f9e827ab-29aa-4a50-92a1-3688c6a4b9fe"))
        {
            Name = "Faxe Kondi",
            HiringDate = new DateOnly(2023, 5, 1)
        };
        
        var sut = new EmployeeXmlFormatter();
        
        // Act
        string result = sut.Format(employee);
        
        // Assert
        // language=xml
        var expectedXml = """
                          <?xml version="1.0" encoding="utf-16"?>
                          <Employee xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                            <Id>f9e827ab-29aa-4a50-92a1-3688c6a4b9fe</Id>
                            <Name>Faxe Kondi</Name>
                            <HiringDate>2023-05-01T00:00:00</HiringDate>
                          </Employee>
                          """;
        result.Should().BeEquivalentTo(expectedXml);
    }
}