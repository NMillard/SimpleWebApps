using ExtensionByClasses.Domain.Formatters.Dynamic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ExtensionByClasses.Domain.Tests.Formatters.Dynamic;

public class EmployeeFormatterManagerShould
{
    [Fact]
    public void ContainAddedFormatters()
    {
        // Arrange
        var json = Substitute.For<IEmployeeFormatter>();
        json.FormattingType.Returns("Json");
        
        var xml = Substitute.For<IEmployeeFormatter>();
        xml.FormattingType.Returns("Xml");
        
        EmployeeFormatterManager sut = new EmployeeFormatterManager(NullLogger<EmployeeFormatterManager>.Instance)
            .Add(json).Add(xml);

        // Act
        IReadOnlyList<string> result = sut.GetAvailableFormats();
        
        // Assert
        result.Should().Contain(["Xml", "Json"]);
    }

    [Theory]
    [InlineData("First", "first content")]
    [InlineData("Second", "second content")]
    public void PickFormatterTypeForRequestedFormat(string format, string expected)
    { 
        // Arrange
        var first = Substitute.For<IEmployeeFormatter>();
        first.FormattingType.Returns("First");
        first.Format(Arg.Any<Employee>()).Returns("first content");
        
        var second = Substitute.For<IEmployeeFormatter>();
        second.FormattingType.Returns("Second");
        second.Format(Arg.Any<Employee>()).Returns("second content");
        
        EmployeeFormatterManager sut = new EmployeeFormatterManager(NullLogger<EmployeeFormatterManager>.Instance)
            .Add([first, second]);

        var employee = new Employee
        {
            Name = "name",
            HiringDate = DateOnly.MinValue
        };
        
        // Act
        string result = sut.Format(employee, format);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}