using Microsoft.AspNetCore.Mvc.Testing;

namespace AdvancedTesting.Demo;

public class WebAppTests
{
    [Fact]
    public async Task SimpleEndpointCall()
    {
        var factory = new WebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        
        HttpResponseMessage response = await client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        WeatherForecast[]? forecast = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
    }

    [Fact]
    public async Task ChangeSettings()
    {
        var factory = new WebApplicationFactory<Program>();
        factory.WithWebHostBuilder(builder =>
        {
            var s = builder.GetSetting("MySettings:Demo");
            builder.UseSetting("MySettings:Demo", "Changed!");
        });
        
        HttpClient client = factory.CreateClient();
        
        HttpResponseMessage response = await client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        WeatherForecast[]? forecast = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
    }
}