using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers {
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase {
        private readonly UserRepository userRepository;
        private readonly IConfiguration configuration;

        public UsersController(UserRepository userRepository, IConfiguration configuration) {
            this.userRepository = userRepository;
            this.configuration = configuration;
        }

        [HttpPost("")]
        public IActionResult Create(User user) {
            userRepository.Create(user);
            return Ok();
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult Get(int id) {
            User user = userRepository.Get(id);
            return Ok(user);
        }

        // Just some stupid login function that has absolutely no real security.
        [HttpGet("{userId:int}")]
        public IActionResult Login(int userId) {
            string privateKey = configuration["Auth:PrivateKey"];

            var credentials = new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
                algorithm: SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim> {
                new Claim("userId", userId.ToString())
            };

            var jwt = new JwtSecurityToken(
                new JwtHeader(credentials),
                new JwtPayload(claims: claims)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { Token = token });
        }
    }
}