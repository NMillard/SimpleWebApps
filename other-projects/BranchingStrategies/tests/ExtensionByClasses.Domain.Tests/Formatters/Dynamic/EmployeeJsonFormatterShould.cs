using ExtensionByClasses.Domain.Formatters.Dynamic;
using FluentAssertions;

namespace ExtensionByClasses.Domain.Tests.Formatters.Dynamic;

public class EmployeeJsonFormatterShould
{
    [Fact]
    public void FormatEmployee()
    {
        var employee = new Employee(Guid.Parse("f9e827ab-29aa-4a50-92a1-3688c6a4b9fe"))
        {
            Name = "Faxe Kondi",
            HiringDate = new DateOnly(2023, 5, 1)
        };

        var sut = new EmployeeJsonFormatter();
        
        // Act
        string result = sut.Format(employee);
        
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
}