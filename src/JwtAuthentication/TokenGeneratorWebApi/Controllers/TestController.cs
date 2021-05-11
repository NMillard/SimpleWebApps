using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TokenGeneratorWebApi.Configurations;

namespace TokenGeneratorWebApi.Controllers {
    [Route("api/[controller]")]
    public class TestController : ControllerBase {
        private readonly SecurityOptions options;

        public TestController(SecurityOptions options) {
            this.options = options;
        }

        [HttpGet(nameof(GenerateManually))]
        public async Task<IActionResult> GenerateManually() {
            using var rsa = RSA.Create();
            string key = await System.IO.File.ReadAllTextAsync(options.RsaPrivateKeyPath);
            rsa.FromXmlString(key);

            DateTimeOffset date = DateTimeOffset.UtcNow;
            long inSeconds = date.ToUnixTimeSeconds();

            string headerText = JsonSerializer.Serialize(new { alg = SecurityAlgorithms.RsaSha256, typ = "JWT" });
            string payloadText = JsonSerializer.Serialize(new {
                nbf = inSeconds,
                exp = inSeconds + 10,
                iat = inSeconds,
                aud = "webapi",
                iss = "webapi"
            }); // nbf = not before, exp = expires, iat = issued at
            string convertedHeader = Base64UrlEncoder.Encode(headerText);
            string convertedPayload = Base64UrlEncoder.Encode(payloadText);

            string manualJwt = $"{convertedHeader}.{convertedPayload}"; // Construct JWT

            byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(manualJwt), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string base64Signature = Base64UrlEncoder.Encode(signature);

            string jwt = $"{convertedHeader}.{convertedPayload}.{base64Signature}";

            return Ok(new { Token = jwt });
        }

        [HttpGet(nameof(Generate))]
        public async Task<IActionResult> Generate() {
            using var rsa = RSA.Create();
            string key = await System.IO.File.ReadAllTextAsync(options.RsaPrivateKeyPath);
            rsa.FromXmlString(key);

            var credentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256
            );

            var jwt = new JwtSecurityToken(
                new JwtHeader(credentials),
                new JwtPayload(
                    issuer: "webapi",
                    audience: "webapi",
                    new List<Claim>(),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(5))
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Test() {
            return Ok();
        }
    }
}