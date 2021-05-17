using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
            var rsa = RSA.Create();
            string key = await System.IO.File.ReadAllTextAsync(options.PrivateKeyFilePath);
            rsa.FromXmlString(key);
            
            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
            
            var jwt = new JwtSecurityToken(
                new JwtHeader(credentials),
                new JwtPayload(
                    "webapi",
                    "webapi",
                    new List<Claim>(),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(3)
                )
            );
            
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return Ok(new { Token = token });
        }
        
        [HttpGet(nameof(GenerateSymmetric))]
        public async Task<IActionResult> GenerateSymmetric() {
            string key = options.SymmetricKey;
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);
            
            var jwt = new JwtSecurityToken(
                new JwtHeader(credentials),
                new JwtPayload(
                    "webapi",
                    "webapi",
                    new List<Claim>(),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(3)
                )
            );
            
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet(nameof(Verify))]
        public async Task<IActionResult> Verify() {
            return Ok();
        }
        
        [Authorize(AuthenticationSchemes = "symm")]
        [HttpGet(nameof(VerifySymmetric))]
        public async Task<IActionResult> VerifySymmetric() {
            return Ok();
        }
    }
}