using Newtonsoft.Json;

namespace Plugin.CountriesModule; 

internal class CountriesRepository {
    public IEnumerable<Country> GetAll() {
        string json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "countries.json"));
        return JsonConvert.DeserializeObject<Country[]>(json);
    }
}