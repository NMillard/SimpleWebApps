using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Samples;

namespace RepositoryPattern {
    public static class ServiceInjectionExtensions {
        public static IServiceCollection AddRepositories(this IServiceCollection services) {
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddEntityFrameworkInMemoryDatabase() // just for demo purposes.
                .AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}