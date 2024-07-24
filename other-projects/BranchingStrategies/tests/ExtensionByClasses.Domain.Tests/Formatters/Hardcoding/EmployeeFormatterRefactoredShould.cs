using ExtensionByClasses.Domain.Formatters.Hardcoding;

namespace ExtensionByClasses.Domain.Tests.Formatters.Hardcoding;

public class EmployeeFormatterRefactoredShould
{
    [Fact]
    public void FormatEmployeeAsPdf()
    {
        var formatter = new PdfFormatter();
        var employee = new Employee
        {
            Name = "Faxe Kondi",
            HiringDate = new DateOnly(2023, 5, 20)
        };
        
        var sut = new EmployeeFormatterRefactored(null, null, formatter);
        
        // Act
        string result = sut.Format(employee, OutputFormat.Pdf);
        
        // Assert
        // some asserts
    }
}