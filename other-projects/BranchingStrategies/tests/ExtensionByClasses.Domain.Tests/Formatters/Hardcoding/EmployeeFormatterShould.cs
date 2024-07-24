using ExtensionByClasses.Domain.Formatters.Hardcoding;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ExtensionByClasses.Domain.Tests.Formatters.Hardcoding;

/// <summary>
/// Testing classes and methods that use hardcoded branches has the derivative effect that
/// you also have to revisit tests and extend those.
///
/// Bonus:
///     Notice how I'm setting up each test by creating the SUT and employee - which I could easily have made into
///     fields for the test class. When writing tests, I've come to prefer having everything explicitly stated
///     in each single test rather than attempting to do some clever setup. It's easier to read and
///     maintain in that way. 
/// </summary>
public class EmployeeFormatterShould
{
    [Fact]
    public void ListAllAvailableTypes()
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<AvaiableFormatOptions>>();
        options.CurrentValue.Returns(new AvaiableFormatOptions
        {
            EmployeeFormats = ["Json", "Xml"]
        });
        var sut = new EmployeeFormatter(options);

        // Act
        IReadOnlyList<string> result = sut.GetAvailableFormats();

        // Assert
        result.Should()
            .Contain([
                "Json",
                "Xml",
                // "Csv" <- add if we were to extend the formats available to us.
            ]);
    }

    [Fact]
    public void FormatEmployeeAsJson()
    {
        // Arrange
        var employee = new Employee(Guid.Parse("f9e827ab-29aa-4a50-92a1-3688c6a4b9fe"))
        {
            Name = "Faxe Kondi",
            HiringDate = new DateOnly(2023, 5, 1)
        };
        
        var options = Substitute.For<IOptionsMonitor<AvaiableFormatOptions>>();
        options.CurrentValue.Returns(new AvaiableFormatOptions
        {
            EmployeeFormats = ["Json", "Xml"]
        });
        
        var sut = new EmployeeFormatter(options);
        
        // Act
        string result = sut.Format(employee, OutputFormat.Json);

        // Assert
        // language=json <- allows highlighting in JetBrains IDEs
        const string expectedJson = """ 
                                    {
                                      "id": "f9e827ab-29aa-4a50-92a1-3688c6a4b9fe",
                                      "name": "Faxe Kondi",
                                      "hiringDate": "2023-05-01"
                                    }
                                    """;
        result.Should()
            .BeEquivalentTo(expectedJson);
    }

    [Fact]
    public void FormatEmployeeAsXml()
    {
        var employee = new Employee(Guid.Parse("f9e827ab-29aa-4a50-92a1-3688c6a4b9fe"))
        {
            Name = "Faxe Kondi",
            HiringDate = new DateOnly(2023, 5, 1)
        };
        
        var options = Substitute.For<IOptionsMonitor<AvaiableFormatOptions>>();
        options.CurrentValue.Returns(new AvaiableFormatOptions
        {
            EmployeeFormats = ["Json", "Xml"]
        });
        
        var sut = new EmployeeFormatter(options);
        
        // Act
        string result = sut.Format(employee, OutputFormat.Xml);
        
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