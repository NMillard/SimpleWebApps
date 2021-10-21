using Microsoft.Extensions.DependencyInjection;
using Plugin.Domain.Abstractions;
using Plugin.Module;

namespace Plugin.WeatherModule; 

public class WeatherModuleInjector : IModuleInstaller {
    public void ConfigureServices(IServiceCollection services) {
        services.AddScoped<IWeatherQuery, SimpleWeatherQuery>()
            .AddSingleton<GoogleOptions>(_ => new GoogleOptions {Page = "https://google.dk"})
            .AddScoped<IAction, DisplayWeather>()
            .AddScoped<IAction, DisplayGoogle>();
    }
}