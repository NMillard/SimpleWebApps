using Riok.Mapperly.Abstractions;

namespace AdvancedTesting.Demo;

public class MappingTests
{
    [Fact]
    public void DemoMapperly()
    {
        var valuation = new Valuation
        {
            Name = "MyName",
            ValuatorEmail = "my@test.dk",
            Price = 10_000
        };
        
        var mapper = new ValuationMapper();
        ValuationResponse valuationResponse = mapper.Map(valuation);
    }
}

[Mapper]
public partial class ValuationMapper
{
    [MapProperty(nameof(Valuation.ValuatorEmail), nameof(ValuationResponse.Email))]
    public partial ValuationResponse Map(Valuation valuation);
}

public class Valuation
{
    public string Name { get; set; }
    public string ValuatorEmail { get; set; }
    public decimal Price { get; set; }
}

public class ValuationResponse
{
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Price { get; set; }
}