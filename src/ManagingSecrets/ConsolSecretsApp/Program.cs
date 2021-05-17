using System;
using System.IO;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace ConsolSecretsApp {
    class Program {
        public static void Main(string[] args) {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT")}.json")
                .AddUserSecrets<Program>()
                .AddAzureKeyVault(
                    new Uri($"https://{Environment.GetEnvironmentVariable("AZ_KEYVAULT")}.vault.azure.net/"),
                    new DefaultAzureCredential())
                .Build();

            Console.WriteLine(config.GetConnectionString("Sql"));
        }
    }
}