namespace ReflectiveProgramming.Demo.UsingInterface;

public class InterfaceDemo
{
    [Fact]
    public void Test2()
    {
        var employee = new Employee();
        var service = new CsvService();

        string result = service.CreateCsv(new List<ICsvRecord> { employee });
    }
}

public interface ICsvRecord
{
    public string ToCsvRecord();
}


public record Employee: ICsvRecord
{
    // Properties omitted
    
    public string ToCsvRecord()
    {
        // Create CSV string from properties
        throw new NotImplementedException();
    }
}


public class CsvService {
    public string CreateCsv(List<ICsvRecord> records)
    {
        // Iterate over each employee calling ".ToCsvRecord()"
        return "complete csv string";
    }
}