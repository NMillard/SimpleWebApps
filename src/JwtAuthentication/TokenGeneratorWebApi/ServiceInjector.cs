using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TokenGeneratorWebApi.Configurations;

namespace TokenGeneratorWebApi {
    public static class ServiceInjector {
        public static IServiceCollection AddRSAKey(this IServiceCollection services, Action<SecurityOptions> config) {
            var options = new SecurityOptions();
            config(options);

            services.AddSingleton(options);

            string keysDirectory = Path.GetDirectoryName(options.RsaPrivateKeyPath);
            if (!Directory.Exists(keysDirectory)) Directory.CreateDirectory(keysDirectory);
            
            using var rsa = RSA.Create();
            using FileStream privateKeyFile = File.Create(options.RsaPrivateKeyPath);
            using FileStream publicKeyFile = File.Create(options.RsaPublicKeyPath);

            string privateKey = rsa.ToXmlString(true);
            string publicKey = rsa.ToXmlString(false);
            privateKeyFile.Write(Encoding.UTF8.GetBytes(privateKey));
            publicKeyFile.Write(Encoding.UTF8.GetBytes(publicKey));
            
            privateKeyFile.Close();
            publicKeyFile.Close();

            return services;
        }
    }
}