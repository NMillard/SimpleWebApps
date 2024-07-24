using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ExtensionByClasses.Domain.Tests.Conditions;

public class TransformProcessShould
{
    [Fact]
    public async Task Demo()
    {
        // Arrange
        var sut = new MyTransformationProcess();

        sut.AddCondition(new FileCheckCondition())
            .AddCondition(new SomethingCondition());
        
        // Act
        await sut.ExecuteAsync();
    }

    [Fact]
    public async Task UsingDependencyInjection()
    {
        IServiceCollection services = new ServiceCollection()
            .AddScoped<IPrecondition, SomethingCondition>()
            .AddScoped<IPrecondition, FileCheckCondition>()
            .AddScoped<IPrecondition, OtherCondition>()
            .AddScoped(provider =>
            {
                List<string> requiredConditions = [nameof(SomethingCondition), nameof(OtherCondition)];
                IEnumerable<IPrecondition> file = provider.GetServices<IPrecondition>()
                    .Where(pc => requiredConditions.Contains(pc.GetType().Name));

                var transformProcess = new MyTransformationProcess();
                foreach (IPrecondition precondition in file) transformProcess.AddCondition(precondition);
                
                return transformProcess;
            });

        var sut = services.BuildServiceProvider().GetRequiredService<TransformProcessBase>();

        await sut.ExecuteAsync();
    }
    
    private class MyTransformationProcess : TransformProcessBase
    {
        protected override Task Execute()
        {
            return Task.CompletedTask;
        }
    }

    private class OtherCondition : IPrecondition
    {
        public bool IsSatisfied(ConditionContext context)
        {
            return true;
        }
    }

    private class SomethingCondition : IPrecondition
    {
        public bool IsSatisfied(ConditionContext context)
        {
            return false;
        }
    }

    private class FileCheckCondition : IPrecondition
    {
        public bool IsSatisfied(ConditionContext context)
        {
            string file = Path.Join("Conditions", "Resources", "file.txt");
            bool exists = File.Exists(file);
            if (!exists) context.AddError(new ConditionError(this, $"{file} doesn't exist!"));
            
            return exists;
        }
    }
}

public class ConditionCheckerShould
{
    [Fact]
    public void TrueWhenAllConditionsAreSatisfied()
    {
        var context = new ConditionContext();
        
        var condition = Substitute.For<IPrecondition>();
        condition.IsSatisfied(context).Returns(true);

        ConditionChecker sut = new ConditionChecker().Add(condition);

        // Act
        bool result = sut.AllConditionsSatisfied(context);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ContainErrorsIfAnyConditionsFail()
    {
        var context = new ConditionContext();
        
        const string errorMessage = "My error!";
        var condition = Substitute.For<IPrecondition>();
        condition.IsSatisfied(Arg.Do<ConditionContext>(c =>
            {
                c.AddError(new ConditionError(condition, errorMessage));
            }))
            .Returns(false);

        ConditionChecker sut = new ConditionChecker().Add(condition);

        // Act
        bool result = sut.AllConditionsSatisfied(context);
        
        // Assert
        result.Should().BeFalse();
        context.Errors.Should().Contain(e => e.Condition.Equals(condition));
        context.Errors.Should().Contain(e => e.ErrorMessage.Equals(errorMessage));
    }
}

public abstract class TransformProcessBase
{
    private readonly ConditionChecker conditionChecker = new();
    
    public async Task ExecuteAsync()
    {
        var context = new ConditionContext();
        if (!conditionChecker.AllConditionsSatisfied(context))
        {
            return;
        }

        await Execute();
    }

    protected abstract Task Execute();
    
    public TransformProcessBase AddCondition(IPrecondition precondition)
    {
        conditionChecker.Add(precondition);
        return this;
    }
}

public class ConditionChecker
{
    private readonly List<IPrecondition> conditions = [];
    
    public ConditionChecker Add(IPrecondition precondition)
    {
        conditions.Add(precondition);
        return this;
    }

    public bool AllConditionsSatisfied(ConditionContext context)
    {
        var allSatisfied = true;
        foreach (IPrecondition precondition in conditions)
        {
            bool isSatisfied = precondition.IsSatisfied(context);
            if (!isSatisfied) allSatisfied = false;
        }

        return allSatisfied;
    }
}

public interface IPrecondition
{
    bool IsSatisfied(ConditionContext context);
}

public record ConditionContext
{
    private readonly List<ConditionError> errors = [];
    
    public IEnumerable<ConditionError> Errors => errors;
    public void AddError(ConditionError error) => errors.Add(error);
}

public record ConditionError(IPrecondition Condition, string ErrorMessage);