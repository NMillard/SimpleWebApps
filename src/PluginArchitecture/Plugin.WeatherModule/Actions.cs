using Newtonsoft.Json;
using Plugin.Domain;
using Plugin.Domain.Abstractions;

namespace Plugin.WeatherModule;

[ActionMeta("display-weather")]
internal class DisplayWeather : IAction {
    public void Action() {
        var weather = new {
            Visibility = "Good"
        };

        Console.WriteLine(JsonConvert.SerializeObject(weather));
    }
}

[ActionMeta("display-google")]
internal class DisplayGoogle : IAction {
    private readonly GoogleOptions options;

    public DisplayGoogle(GoogleOptions options) => this.options = options;

    public void Action() {
        var client = new HttpClient();
        string content = client.GetStringAsync(options.Page).GetAwaiter().GetResult();

        Console.WriteLine(content);
    }
}