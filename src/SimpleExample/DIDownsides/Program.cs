using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

var container2 = new ServiceCollection()
    .AddScoped<IRepository<User>, UserRepository>()
    .AddScoped<IRepository<Article>, ArticleRepository>()
    .AddScoped<ICache<User>, UserCache>()
    .AddScoped<ICache<Article>, ArticleCache>()
    .AddScoped<ILogicDumpster, OtherService>()
    .AddScoped<BusinessSettings>()
    .AddScoped<IBusinessRule, UserNameValidation>()
    .AddScoped<IBusinessRule, InsanePasswordRequirement>()
    .AddScoped<IBusinessRule, SkipInsanePasswordRequirement>()
    .AddScoped<DomainExceptions>()
    .AddScoped<ExceptionThrowerService>()
    .AddScoped<MuchBetterService>()
    .BuildServiceProvider();

var bestServiceEver = container2.GetRequiredService<MuchBetterService>();

bestServiceEver.GreetUser(Guid.Empty);


ServiceProvider container = new ServiceCollection()
    .AddScoped<IRepository<User>, UserRepository>()
    .AddScoped<IRepository<Article>, ArticleRepository>()
    .AddScoped<ICache<User>, UserCache>()
    .AddScoped<ICache<Article>, ArticleCache>()
    .AddScoped<ILogicDumpster, OtherService>()
    .AddScoped<BusinessSettings>()
    .AddScoped<IBusinessRule, UserNameValidation>()
    .AddScoped<IBusinessRule, InsanePasswordRequirement>()
    .AddScoped<IBusinessRule, SkipInsanePasswordRequirement>()
    .AddScoped<DomainExceptions>()
    .AddScoped<ExceptionThrowerService>()
    .AddScoped<MuchBetterService>()
    .BuildServiceProvider();

MyStupidService service = container.GetRequiredService<MyStupidService>();

service.DoThings();


public class MyStupidService {
    private readonly ITime time;
    private readonly IRepository<User> userRepository;
    private readonly IRepository<Article> articleRepository;
    private readonly ICache<User> userCache;
    private readonly ICache<Article> articleCache;
    private readonly IEnumerable<IBusinessRule> rules;
    private readonly ExceptionThrowerService throwerService;
    private readonly BusinessSettings settings;

    public MyStupidService() {
        
    }
    
    public MyStupidService(
        ITime time,
        IRepository<User> userRepository,
        IRepository<Article> articleRepository,
        ICache<User> userCache,
        ICache<Article> articleCache,
        ILogicDumpster logicDumpster,
        IEnumerable<IBusinessRule> rules,
        ExceptionThrowerService throwerService,
        BusinessSettings businessSettings
    ) {
        this.time = time;
        this.userRepository = userRepository;
        this.articleRepository = articleRepository;
        this.userCache = userCache;
        this.articleCache = articleCache;
        this.rules = rules;
        this.throwerService = throwerService;
        this.settings = settings;
    }

    public void DoThings() {
        // things...
    }
}


public class MuchBetterService {
    private readonly ITime time;
    private readonly IRepository<User> userRepository;
    private readonly IRepository<Article> articleRepository;
    private readonly ICache<User> userCache;
    private readonly ICache<Article> articleCache;
    private readonly IEnumerable<IBusinessRule> rules;
    private readonly ExceptionThrowerService throwerService;
    private readonly BusinessSettings settings;
    
    public MuchBetterService(
        ITime time,
        IRepository<User> userRepository,
        IRepository<Article> articleRepository,
        ICache<User> userCache,
        ICache<Article> articleCache,
        ILogicDumpster logicDumpster,
        IEnumerable<IBusinessRule> rules,
        ExceptionThrowerService throwerService,
        BusinessSettings settings
    ) {
        this.time = time;
        this.userRepository = userRepository;
        this.articleRepository = articleRepository;
        this.userCache = userCache;
        this.articleCache = articleCache;
        this.rules = rules;
        this.throwerService = throwerService;
        this.settings = settings;
    }

    public void GreetUser(Guid userId) => Console.WriteLine($"Hello {userId}");

    public void PerformActionRequiring10Dependencies() { }

    public TimeOnly WhatsTheTime() => TimeOnly.FromDateTime(DateTime.UtcNow);
}


public interface IRepository<T> {
    Task<IEnumerable<T>> GetAsync();
}

public interface ICache<T> {
    Task<bool> TryGetAsync(out IEnumerable<T> users);
}

public class UserRepository : IRepository<User> {
    public Task<IEnumerable<User>> GetAsync() {
        throw new NotImplementedException();
    }
}

public class ArticleRepository : IRepository<Article> {
    public Task<IEnumerable<Article>> GetAsync() {
        throw new NotImplementedException();
    }
}

public class UserCache : ICache<User> {
    public Task<bool> TryGetAsync(out IEnumerable<User> users) {
        throw new NotImplementedException();
    }
}

public class ArticleCache : ICache<Article> {
    public Task<bool> TryGetAsync(out IEnumerable<Article> users) {
        throw new NotImplementedException();
    }
}

public interface ILogicDumpster { }

public class OtherService : ILogicDumpster { }

public interface ITime {
    TimeOnly GetUtcTime();
}

public class Now : ITime {
    public TimeOnly GetUtcTime() => TimeOnly.FromDateTime(DateTime.UtcNow);
}

public interface IExceptionThrower {
    /// <summary>
    /// Throws <see cref="TException"/>.
    /// </summary>
    void Throw<TException>() where TException : Exception, new();
}

public class DomainExceptions { }

public class ExceptionThrowerService : IExceptionThrower {
    private readonly DomainExceptions domainExceptions;

    public ExceptionThrowerService(DomainExceptions domainExceptions) {
        this.domainExceptions = domainExceptions;
    }

    public void Throw<TException>() where TException : Exception, new()
        => throw new TException();
}

public class User { }

public class Article { }

public class BusinessSettings { }

public interface IBusinessRule { }

public class UserNameValidation : IBusinessRule { }

public class InsanePasswordRequirement : IBusinessRule { }

public class SkipInsanePasswordRequirement : IBusinessRule { }