namespace BusinessRules.Demo.ServiceEvaluation;

public class Demo
{
    [Theory]
    [MemberData(nameof(Usernames))]
    public void Test1(string value, bool expected)
    {
        var sut = new UserService();

        // Act
        bool result = sut.IsUsernameValid(new Username(value));

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> Usernames()
    {
        return new List<object[]>()
        {
            new object[] { "a", false },
            new object[] { "", false },
            new object[] { null!, false },
            new object[] { "abc*", false },
            new object[] { "abc", true },
        };
    }
}

public class UserService
{
    public bool IsUsernameValid(Username username)
    {
        return username.Value switch
        {
            null => false,
            "" => false,
            { Length: > 50 } => false,
            { Length: < 3 } => false,
            var value when value.Any(v => !char.IsLetterOrDigit(v)) => false,
            _ => true
        };
    }
}

public record Username(string Value);