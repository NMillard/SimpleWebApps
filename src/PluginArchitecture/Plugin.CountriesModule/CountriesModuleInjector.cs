using Microsoft.Extensions.DependencyInjection;
using Plugin.Domain.Abstractions;
using Plugin.Module;

namespace Plugin.CountriesModule;

public class CountriesModuleInjector : IModuleInstaller {
    public void ConfigureServices(IServiceCollection services) {
        services.AddScoped<CountriesRepository>()
            .AddScoped<IAction, DisplayCountries>();
    }
}