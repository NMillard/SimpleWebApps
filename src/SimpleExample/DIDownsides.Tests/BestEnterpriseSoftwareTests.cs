using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;


public class BestEnterpriseSoftwareTests {
    private readonly IServiceCollection container;

    public BestEnterpriseSoftwareTests() {
        container = new ServiceCollection()
            .AddScoped<ITime, Now>()
            .AddScoped<IRepository<User>, UserRepository>()
            .AddScoped<IRepository<Article>, ArticleRepository>()
            .AddScoped<ICache<User>, UserCache>()
            .AddScoped<ICache<Article>, ArticleCache>()
            .AddScoped<ILogicDumpster, OtherService>()
            .AddScoped<BusinessSettings>()
            .AddScoped<IBusinessRule, UserNameValidation>()
            .AddScoped<IBusinessRule, InsanePasswordRequirement>()
            .AddScoped<IBusinessRule, SkipInsanePasswordRequirement>()
            .AddScoped<ExceptionThrowerService>()
            .AddScoped<MuchBetterService>();
    }

    [Fact]
    public void MuchBetterWithDI() {
        var sut = container.BuildServiceProvider().GetRequiredService<MuchBetterService>();
        sut.GreetUser(Guid.Empty);
    }
    
    
    [Fact]
    public void TestingTightlyCoupledClass() {
        var sut = new MyStupidService();
        sut.DoThings();
    }

    [Fact]
    public void StupidClassTest() {
        var sut = new MyStupidService();
    }

    public void MuchBetterClassTestNewingUp() {
        var sut = new MuchBetterService(
            new Now(),
            new UserRepository(),
            new ArticleRepository(),
            new UserCache(),
            new ArticleCache(),
            new OtherService(),
            new List<IBusinessRule> {
                new InsanePasswordRequirement(), new UserNameValidation(), new SkipInsanePasswordRequirement()
            },
            new ExceptionThrowerService(new DomainExceptions()),
            new BusinessSettings());

        sut.PerformActionRequiring10Dependencies();
    }

    [Fact]
    public void MuchBetterWithNulls() {
        var mock = new Mock<ITime>();
        mock.Setup(t => t.GetUtcTime()).Returns(new TimeOnly(10, 00));

        var sut = new MuchBetterService(
            mock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        TimeOnly result = sut.WhatsTheTime();

        Assert.Equal(new TimeOnly(10, 00), result);
    }
}

public class AutoMockInjector<T> {
    public T GetConstructedService { get; private set; }
    
    public AutoMockInjector<T> ReplaceMockedService<TService>(Func<TService> mockFactory) {
        /* WOLOLO! */
        return this;
    }
    
    public AutoMockInjector<T> ConstructWithMocks() {
        /* hokus pokus implementation */
        return this;
    }
}

public class Test {
    [Fact]
    public void AutoMocker() {
        MuchBetterService sut = new AutoMockInjector<MuchBetterService>().ConstructWithMocks()
            .ReplaceMockedService<ITime>(() => {
                var mock = new Mock<ITime>();
                mock.Setup(t => t.GetUtcTime()).Returns(new TimeOnly(10, 00));

                return mock.Object;
            })
            .GetConstructedService;
        
        TimeOnly result = sut.WhatsTheTime();

        Assert.Equal(new TimeOnly(10, 00), result);
    }
}









