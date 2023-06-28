using Microsoft.Extensions.DependencyInjection;

namespace BusinessRules.Demo.DynamicBusinessRules;

public class Demo
{
    [Theory]
    [MemberData(nameof(Usernames))]
    public void First(string value, bool expected)
    {
        var repository = new InMemoryUsernameConfigurationRepository();
        var usernamePolicies = new List<IUsernameRule>
        {
            new MaxLengthRule(),
            new MinLengthRule(),
            new OnlyAlphanumericCharacters()
        };

        var sut = new UsernameEvaluator(usernamePolicies, repository);

        bool result = sut.Evaluate(new Username(value));

        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(Usernames))]
    public void Second(string value, bool expected)
    {
        ServiceProvider services = new ServiceCollection()
            .AddScoped<IUsernameRule, MaxLengthRule>()
            .AddScoped<IUsernameRule, MinLengthRule>()
            .AddScoped<IUsernameRule, OnlyAlphanumericCharacters>()
            .AddScoped<UsernameEvaluator>()
            .AddScoped<IUsernameConfigurationRepository, InMemoryUsernameConfigurationRepository>()
            .BuildServiceProvider();

        var sut = services.GetRequiredService<UsernameEvaluator>();

        bool result = sut.Evaluate(new Username(value));

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> Usernames()
    {
        return new List<object[]>
        {
            new object[] { "a", false },
            new object[] { "", false },
            new object[] { null!, false },
            new object[] { "abc", true },
            new object[] { "12345678901234567890", true },
            new object[] { "12345678901234567890", true },
            new object[] { "hello*", true },
        };
    }
}

public record Username(string Value);

public class UsernameEvaluator
{
    public UsernameEvaluator(
        IEnumerable<IUsernameRule> rules,
        IUsernameConfigurationRepository repository)
    {
        this.rules = rules;
        this.repository = repository;
    }

    private readonly IEnumerable<IUsernameRule> rules;
    private readonly IUsernameConfigurationRepository repository;

    public bool Evaluate(Username username)
    {
        if (string.IsNullOrEmpty(username.Value)) return false;

        List<UsernameConfiguration> configurations = repository.GetAll()
            .Where(uc => uc.IsActive)
            .ToList();

        List<IUsernameRule> activeRules = rules
            .Where(p => configurations.Any(c => c.Key.Equals(p.GetType().Name) && c.IsActive))
            .ToList();

        return activeRules.All(p =>
        {
            UsernameConfiguration? config = repository.GetByName(p.GetType().Name);
            return p.Evaluate(username, new RuleContext { Parameters = config?.Parameters });
        });
    }
}

public class RuleContext
{
    public bool IsPremium { get; set; }
    public object? Parameters { get; set; }
}


public interface IUsernameRule
{
    bool Evaluate(Username username, RuleContext context);
}


public class MaxLengthRule : IUsernameRule
{
    public bool Evaluate(Username username, RuleContext context)
    {
        if (context.Parameters is not LengthParameters parameters) return false;
        
        return username.Value.Length <= parameters.Length;
    }
}

public class MinLengthRule : IUsernameRule
{
    public bool Evaluate(Username username, RuleContext context)
    {
        if (context.Parameters is not LengthParameters parameters) return false;
        
        return username.Value.Length >= parameters.Length;
    }
}

public class OnlyAlphanumericCharacters : IUsernameRule
{
    public bool Evaluate(Username username, RuleContext context) 
        => username.Value.All(char.IsLetterOrDigit);
}

public interface IUsernameConfigurationRepository
{
    UsernameConfiguration? GetByName(string policyName);
    IEnumerable<UsernameConfiguration> GetAll();
}

public class InMemoryUsernameConfigurationRepository : IUsernameConfigurationRepository
{
    private List<UsernameConfiguration> UsernameConfigurations = new()
    {
        new UsernameConfiguration { Key = nameof(MinLengthRule), Parameters = new LengthParameters { Length = 3 } },
        new UsernameConfiguration { Key = nameof(MaxLengthRule), Parameters = new LengthParameters { Length = 20 } },
        new UsernameConfiguration { Key = nameof(OnlyAlphanumericCharacters), IsActive = false }
    };

    public UsernameConfiguration? GetByName(string policyName)
        => UsernameConfigurations.SingleOrDefault(uc => uc.Key.Equals(policyName));

    public IEnumerable<UsernameConfiguration> GetAll()
        => UsernameConfigurations.AsReadOnly();
}


public interface IRuleParameters
{
    
}

public class LengthParameters : IRuleParameters
{
    public int Length { get; set; }
}

public class UsernameConfiguration
{
    public required string Key { get; set; }
    public IRuleParameters? Parameters { get; set; }
    public bool IsActive { get; set; } = true;
}