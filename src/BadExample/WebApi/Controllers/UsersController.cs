using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers {
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private readonly UserRepository userRepository;
        private readonly IConfiguration configuration;

        public UsersController(UserRepository userRepository, IConfiguration configuration) {
            this.userRepository = userRepository;
            this.configuration = configuration;
        }

        [HttpPost("")]
        public IActionResult Create([FromBody] User user) {
            userRepository.Create(user);
            return Ok();
        }

        [Authorize]
        [HttpGet("")]
        public IActionResult Get() {
            string userId = HttpContext.User.FindFirstValue("userId");
            User user = userRepository.Get(int.Parse(userId));
            return Ok(user);
        }

        [HttpPost("{userId:int}/upload/profileimage")]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile picture) {
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "staging", userId.ToString());
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            string fileExtension = picture.FileName.Split(".")[^1];
            string fileName = $"{Guid.NewGuid():N}.{fileExtension}";

            var img = new FileInfo(Path.Combine(savePath, fileName));
            FileStream stream = img.OpenWrite();
            await picture.CopyToAsync(stream);
            
            stream.Close();

            return Ok();
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