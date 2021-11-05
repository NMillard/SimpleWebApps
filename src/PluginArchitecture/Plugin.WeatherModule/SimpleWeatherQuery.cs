using Plugin.Domain.Abstractions;

namespace Plugin.WeatherModule; 

public class SimpleWeatherQuery : IWeatherQuery {
    public void Execute() {
        Console.WriteLine("Clear weather...");
    }
}