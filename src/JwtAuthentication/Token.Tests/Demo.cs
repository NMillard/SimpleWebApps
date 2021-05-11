using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Token.Tests {
    public class UnitTest1 {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RSAParameters privateParameters;
        private readonly RSAParameters publicParameters;
        
        public UnitTest1(ITestOutputHelper testOutputHelper) {
            this.testOutputHelper = testOutputHelper;

            using var rsa = RSA.Create();
            privateParameters = rsa.ExportParameters(true);
            publicParameters = rsa.ExportParameters(false);
        }
        
        [Fact]
        public void PrivateAndPublicKeys() {
            using var rsa = RSA.Create(); // Create cryptographic keypair

            RSAParameters privateParams = rsa.ExportParameters(true);
            RSAParameters publicParams = rsa.ExportParameters(true);
            
            string privateKey = rsa.ToXmlString(true); // export private params
            string publicKey = rsa.ToXmlString(false); // export public params

            testOutputHelper.WriteLine($"--- Private key ---{Environment.NewLine}{privateKey}");
            testOutputHelper.WriteLine($"--- Public key ---{Environment.NewLine}{publicKey}");
        }

        [Fact]
        public void LoadPublicKey() {
            using var rsa = RSA.Create(); // our application's keypair
            
            // Create public key from our keypair
            RSAParameters publicParams = rsa.ExportParameters(false);
            using var publicKeyFromParams = RSA.Create(publicParams);

            // Loading from XML
            string publicKeyXml = rsa.ToXmlString(false);
            using var publicKeyFromXml = RSA.Create();
            publicKeyFromXml.FromXmlString(publicKeyXml);
        }

        [Fact]
        public void EncryptDataWithPublicKey() {
            using var publicKey = RSA.Create(publicParameters);

            byte[] plainText = Encoding.UTF8.GetBytes("hello there!");
            byte[] cipherText = publicKey.Encrypt(plainText, RSAEncryptionPadding.Pkcs1);

            testOutputHelper.WriteLine(Encoding.UTF8.GetString(cipherText));
        }

        [Fact]
        public void DecryptDataWithPrivateKey() {
            using var privateKey = RSA.Create(privateParameters);
            using var publicKey = RSA.Create(publicParameters);

            // Encrypt message with public key
            byte[] plainText = Encoding.UTF8.GetBytes("hello there!");
            byte[] cipherText = publicKey.Encrypt(plainText, RSAEncryptionPadding.Pkcs1);

            // Decrypt with private key
            byte[] decryptedText = privateKey.Decrypt(cipherText, RSAEncryptionPadding.Pkcs1);
            testOutputHelper.WriteLine(Encoding.UTF8.GetString(decryptedText));
        }

        [Fact]
        public void SignDataWithPrivateKey() {
            using var privateKey = RSA.Create(privateParameters);

            // Sign message with private key without encryption
            byte[] message = Encoding.UTF8.GetBytes("Not so secret message");
            byte[] signature = privateKey.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            testOutputHelper.WriteLine(Encoding.UTF8.GetString(signature));
        }

        [Fact]
        public void VerifyMessageWithPublicKey() {
            using var privateKey = RSA.Create(privateParameters);
            using var publicKey = RSA.Create(publicParameters);
            
            byte[] message = Encoding.UTF8.GetBytes("Not so secret message");
            byte[] signature = privateKey.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            bool verified = publicKey.VerifyData(message, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            testOutputHelper.WriteLine(verified.ToString());
        }

        [Fact]
        public void TryVerifyDataFromOtherPrivateKey() {
            // Imagine two different people having their own private key
            using var privateKey = RSA.Create(privateParameters);
            using var otherPrivateKey = RSA.Create();
            
            using var publicKey = RSA.Create(publicParameters);
            
            byte[] message = Encoding.UTF8.GetBytes("Not so secret message");
            byte[] signature = otherPrivateKey.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Verify the message came from the person with the private key to our public key
            bool verified = publicKey.VerifyData(message, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            testOutputHelper.WriteLine(verified.ToString());
        }

        [Fact]
        public void CreateJwtManuallyWithoutSignature() {
            DateTimeOffset date = DateTimeOffset.UtcNow;
            long inSeconds = date.ToUnixTimeSeconds();
            
            // Manually construct JWT
            string headerText = JsonSerializer.Serialize(new { alg = "none", typ = "JWT" });
            string payloadText = JsonSerializer.Serialize(new { nbf = inSeconds, exp = inSeconds + 10, iat = inSeconds }); // nbf = not before, exp = expires, iat = issued at

            string convertedHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerText)).Replace("=", "");
            string convertedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadText)).Replace("=", "");
            
            string manualJwt = $"{convertedHeader}.{convertedPayload}."; // Construct JWT
        }

        [Fact]
        public void CreateSignedJwt() {
            using var rsa = RSA.Create(privateParameters);

            DateTimeOffset date = DateTimeOffset.UtcNow;
            long inSeconds = date.ToUnixTimeSeconds();

            string headerText = JsonSerializer.Serialize(new { alg = "RS256", typ = "JWT" });
            string payloadText = JsonSerializer.Serialize(new { nbf = inSeconds, exp = inSeconds + 10, iat = inSeconds }); // nbf = not before, exp = expires, iat = issued at
            string convertedHeader = Base64UrlEncoder.Encode(headerText);
            string convertedPayload = Base64UrlEncoder.Encode(payloadText);
            
            string manualJwt = $"{convertedHeader}.{convertedPayload}"; // Construct JWT

            byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(manualJwt), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string base64Signature = Base64UrlEncoder.Encode(signature);

            string jwt = $"{convertedHeader}.{convertedPayload}.{base64Signature}";
            
            var handler = new JwtSecurityTokenHandler().ValidateToken(jwt, new TokenValidationParameters {
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false
            }, out var s);
        }
    }
}










