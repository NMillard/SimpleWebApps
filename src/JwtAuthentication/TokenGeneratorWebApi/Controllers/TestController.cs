using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
                    claims: new List<Claim>(),
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddHours(5)
                )
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return Ok(new {Token = token});
        }

        [Authorize]
        [HttpGet]
        public IActionResult Test() {
            return Ok();
        }
    }
}