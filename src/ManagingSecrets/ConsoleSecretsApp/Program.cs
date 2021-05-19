using System;
using System.IO;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace ConsoleSecretsApp {
    public class Program {
        public static void Main(string[] args) {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT")}.json")
                .AddUserSecrets<Program>()
                .AddAzureKeyVault(
                    new Uri($"https://{Environment.GetEnvironmentVariable("AZ_KEYVAULT")}.vault.azure.net/"),
                    new DefaultAzureCredential())
                .Build();
            
            Console.WriteLine(config["Something:MyValue"]);
        }
    }
}