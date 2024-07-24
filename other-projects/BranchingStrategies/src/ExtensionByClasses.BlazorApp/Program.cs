using ExtensionByClasses.BlazorApp.Components;
using ExtensionByClasses.Domain.Formatters.Dynamic;
using ExtensionByClasses.Domain.Formatters.Hardcoding;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .Configure<EmployeeFormatterTypes>(
        builder.Configuration.GetSection(nameof(EmployeeFormatterTypes))
    );

builder.Services.Configure<AvaiableFormatOptions>(
    builder.Configuration.GetSection("EmployeeFormatterTypes")
);

builder.Services
    .AddScoped<EmployeeFormatter>()
    .AddSingleton<IEmployeeFormatter, EmployeeXmlFormatter>()
    .AddSingleton<IEmployeeFormatter, EmployeeJsonFormatter>()
    .AddSingleton<IEmployeeFormatter, EmployeePdfFormatter>()
    .AddScoped<FormatterRepository>()
    .AddScoped<EmployeeFormatterManager>(provider =>
    {
        var logger = provider.GetRequiredService<ILogger<EmployeeFormatterManager>>();
        var options = provider.GetRequiredService<IOptionsMonitor<EmployeeFormatterTypes>>();

        IEnumerable<IEmployeeFormatter> formatters = provider.GetServices<IEmployeeFormatter>()
            .Where(ef => options.CurrentValue.EmployeeFormats.Contains(ef.FormattingType));

        return new EmployeeFormatterManager(logger).Add(formatters);
    });

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

internal class EmployeeFormatterTypes
{
    public List<string> EmployeeFormats { get; set; } = [];
}

internal class FormatterRepository(
    IEnumerable<IEmployeeFormatter> formatters,
    IOptionsMonitor<EmployeeFormatterTypes> options)
{
    public IEnumerable<IEmployeeFormatter> GetEnabledFormatters()
    {
        List<string> enabledFormats = options.CurrentValue.EmployeeFormats;
        return formatters.Where(ef => enabledFormats.Contains(ef.FormattingType));
    }
}