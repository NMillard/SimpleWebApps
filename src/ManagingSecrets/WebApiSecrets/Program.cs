using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebApiSecrets {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration(builder => {
                    IConfigurationRoot config = builder.Build();

                    var vaultUri = $"https://{config["KeyVault:Name"]}.vault.azure.net/";
                    builder.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
                });
    }
}