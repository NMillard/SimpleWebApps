namespace StreamingContentApp.RealEstateFeature;

public abstract class RealEstate {
    public RealEstate() {
        Id = Guid.NewGuid();
    }
    
    public Guid Id { get; }
}

public class SingleFamilyHome : RealEstate {
    public string Address { get; init; }
}

public class MultiFamilyHome : RealEstate {
    public string Address { get; set; }
    public int NumberOfFamilies { get; set; }
}