using StreamingContentApp.RealEstateFeature;

namespace StreamingContentApp.Abstractions; 

public interface IRealEstateRepository {
    IAsyncEnumerable<RealEstate> GetAllAsync();
}