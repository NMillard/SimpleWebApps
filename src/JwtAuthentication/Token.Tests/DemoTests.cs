using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Token.Tests {
    public class DemoTests {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RSAParameters privateParameters;
        private readonly RSAParameters publicParameters;
        
        public DemoTests(ITestOutputHelper testOutputHelper) {
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
            using var privateKey = RSA.Create(privateParameters);
            using var publicKey = RSA.Create(publicParameters);

            byte[] plainText = Encoding.UTF8.GetBytes("hello there!");
            byte[] cipherText = publicKey.Encrypt(plainText, RSAEncryptionPadding.Pkcs1);

            testOutputHelper.WriteLine(Encoding.UTF8.GetString(cipherText));
            
            // Decrypt with private key
            byte[] decryptedText = privateKey.Decrypt(cipherText, RSAEncryptionPadding.Pkcs1);
            testOutputHelper.WriteLine(Encoding.UTF8.GetString(decryptedText));
        }
        
        [Fact]
        public void SignDataWithPrivateKeyAndVerifyWithPublicKey() {
            using var privateKey = RSA.Create(privateParameters);
            using var publicKey = RSA.Create(publicParameters);
            
            // Sign message with private key without encryption. The signed data does not need to be encrypted.
            // The SignData actually hashes the message and then signs that hash.
            byte[] message = Encoding.UTF8.GetBytes("Not so secret message");
            byte[] signature = privateKey.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            testOutputHelper.WriteLine(Encoding.UTF8.GetString(signature));
            
            // Use the public key to verify the message was signed with the corresponding private key.
            bool verified = publicKey.VerifyData(message, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            testOutputHelper.WriteLine(verified.ToString());
        }
        
        [Fact]
        public void CreateJwtManuallyWithoutSignature() {
            DateTimeOffset date = DateTimeOffset.UtcNow;
            long inSeconds = date.ToUnixTimeSeconds();
            
            // Manually construct JWT
            string headerJson = JsonSerializer.Serialize(new { alg = "none", typ = "JWT" });
            string payloadJson = JsonSerializer.Serialize(new { nbf = inSeconds, exp = inSeconds + 10, iat = inSeconds }); // nbf = not before, exp = expires, iat = issued at

            string base64Header = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(headerJson));
            string base64Payload = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(payloadJson));

            string manualJwt = $"{base64Header}.{base64Payload}."; // Construct JWT

            testOutputHelper.WriteLine(manualJwt);
        }

        [Fact]
        public void CreateSignedJwt() {
            using var privateRsa = RSA.Create(privateParameters);

            DateTimeOffset date = DateTimeOffset.UtcNow;
            long inSeconds = date.ToUnixTimeSeconds();

            string headerText = JsonSerializer.Serialize(new { alg = "RS256", typ = "JWT" });
            string payloadText = JsonSerializer.Serialize(new { nbf = inSeconds, exp = inSeconds + 10, iat = inSeconds }); // nbf = not before, exp = expires, iat = issued at
            string convertedHeader = Base64UrlEncoder.Encode(headerText);
            string convertedPayload = Base64UrlEncoder.Encode(payloadText);
            
            string manualJwt = $"{convertedHeader}.{convertedPayload}"; // Construct JWT

            // Signing the JWT
            byte[] signature = privateRsa.SignData(Encoding.UTF8.GetBytes(manualJwt), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string base64Signature = Base64UrlEncoder.Encode(signature);

            string jwt = $"{convertedHeader}.{convertedPayload}.{base64Signature}";
            using var otherRsa = RSA.Create();
            // Verifying the signature using our rsa 
            var handler = new JwtSecurityTokenHandler().ValidateToken(jwt, new TokenValidationParameters {
                IssuerSigningKey = new RsaSecurityKey(otherRsa),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            }, out var s);
        }
        
        /// <summary>
        /// Other security stuff...
        /// 
        /// Generate a set of private and public keys.
        ///
        /// 1. Generate the private key
        /// openssl genrsa -out private.key 1024
        /// 
        /// 2. Derive public key from the private one
        /// openssl rsa -in private.key -out public.key -pubout -outform PEM
        ///
        /// 3. Generate root Certificate Authority (CA)
        /// openssl req -x509 -sha256 -new -nodes -key private.key -days 3650 -out ca.crt
        ///
        /// Private key and Certificate Signing Request (CSR) can be created together using
        /// openssl req -out server.csr -new -newkey rsa:2048 -nodes -keyout private.key
        /// or later using an existing private key using:
        /// openssl req -new -out server.csr -key private.key
        ///
        /// Create client certificate using the same command as above to create client private key and CSR, and then
        /// run the command below:
        /// openssl x509 -req -in client.csr -out client.crt -CA ca.crt -CAKey server-private.key -set_serial 01 -days 3650
        ///
        /// You can create pfx from the private key and certificate like this:
        /// openssl pkcs12 -inkey private.key -in ca.crt -export -out server.pfx -passout pass:word
        /// </summary>
        [Fact]
        public void testName() {
            string @private = File.OpenText("Keys/private.key").ReadToEnd();
            string @public = File.OpenText("Keys/public.key").ReadToEnd();
            
            using var privateKey = RSA.Create();
            privateKey.ImportFromPem(new ReadOnlySpan<char>(@private.ToCharArray()));
            
            using var publicKey = RSA.Create();
            publicKey.ImportFromPem(@public.ToCharArray());

            const string message = "Hello there";
            byte[] signature = privateKey.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            testOutputHelper.WriteLine(Encoding.UTF8.GetString(signature));

            bool verified = publicKey.VerifyData(Encoding.UTF8.GetBytes(message), signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            
            byte[] cipher = publicKey.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.Pkcs1);
            byte[] plain = privateKey.Decrypt(cipher, RSAEncryptionPadding.Pkcs1);
            
            testOutputHelper.WriteLine(Encoding.UTF8.GetString(plain));
        }
    }
}










