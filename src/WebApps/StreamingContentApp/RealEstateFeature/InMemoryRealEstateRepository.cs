using StreamingContentApp.Abstractions;

namespace StreamingContentApp.RealEstateFeature;

public class InMemoryRealEstateRepository : IRealEstateRepository {
    public async IAsyncEnumerable<RealEstate> GetAllAsync() {
        yield return new SingleFamilyHome() { Address = "Trolle allé 20 st d, 4000 Køge" };
        yield return new SingleFamilyHome() { Address = "Helges 13 4th, 2000 Frederiksberg" };
        yield return new SingleFamilyHome() { Address = "Grootsi 18, 2670 Karlslunde" };
        yield return new SingleFamilyHome() { Address = "Falkoner Allé 10, 2000 Frederiksberg" };
        yield return new SingleFamilyHome() { Address = "Givegos 1, 8910 Skanderborg" };
        yield return new MultiFamilyHome() { Address = "Gyrosvænget 3, 8999 Tils", NumberOfFamilies = 3};
    }
}