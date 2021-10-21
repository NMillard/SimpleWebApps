using Newtonsoft.Json;
using Plugin.Domain;
using Plugin.Domain.Abstractions;

namespace Plugin.CountriesModule;

[ActionMeta("display-countries")]
internal class DisplayCountries : IAction {
    private readonly CountriesRepository repository;

    public DisplayCountries(CountriesRepository repository) => this.repository = repository;

    public void Action() {
        IEnumerable<Country> countries = repository.GetAll();
        foreach (Country country in countries) Console.WriteLine($"{country.Code} - {country.Name}");
    }
}