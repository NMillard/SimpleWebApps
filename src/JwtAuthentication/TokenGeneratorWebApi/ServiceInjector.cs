using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TokenGeneratorWebApi.Configurations;

namespace TokenGeneratorWebApi {
    public static class ServiceInjector {
        public static IServiceCollection AddRsaKeys(this IServiceCollection services, SecurityOptions options) {
            string keysFolder = Path.GetDirectoryName(options.PrivateKeyFilePath);
            if (!Directory.Exists(keysFolder)) Directory.CreateDirectory(keysFolder);

            var rsa = RSA.Create();
            string privateKeyXml = rsa.ToXmlString(true);
            string publicKeyXml = rsa.ToXmlString(false);

            using var privateFile = File.Create(options.PrivateKeyFilePath);
            using var publicFile = File.Create(options.PublicKeyFilePath);
            
            privateFile.Write(Encoding.UTF8.GetBytes(privateKeyXml));
            publicFile.Write(Encoding.UTF8.GetBytes(publicKeyXml));

            return services;
        }
    }
}