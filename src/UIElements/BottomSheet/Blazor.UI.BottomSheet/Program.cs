using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.UI.BottomSheet;
using Blazor.UI.BottomSheet.Components.BottomSheet.Services;
using Blazor.UI.BottomSheet.Components.Snackbar;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<BottomSheetService>();
builder.Services.AddSingleton<SnackbarService>();

await builder.Build().RunAsync();