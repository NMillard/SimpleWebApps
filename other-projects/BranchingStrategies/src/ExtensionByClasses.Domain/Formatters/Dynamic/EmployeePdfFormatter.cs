using PuppeteerSharp;

namespace ExtensionByClasses.Domain.Formatters.Dynamic;

public class EmployeePdfFormatter : IEmployeeFormatter
{
    public string Format(Employee employee)
    {
        string downloadPath = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local",
            "chrome-headless"
        );
        
        // language=html
        var html = $"""
                   <h1>{employee.Name}</h1>
                   <p>Id: {employee.Id}</p> 
                   <p>Hiring date: {employee.HiringDate.ToString("yyyy MMMM dd")}</p> 
                   """;

        var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
        {
            Path = downloadPath
        });
        browserFetcher.DownloadAsync().Wait();
        using IBrowser browser = Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
        }).Result;
        using IPage page = browser.NewPageAsync().Result;
        
        page.SetContentAsync(html).Wait();
        byte[] bytes = page.PdfDataAsync().Result;
        
        return Convert.ToBase64String(bytes);
    }

    public string FormattingType => "Pdf";
}