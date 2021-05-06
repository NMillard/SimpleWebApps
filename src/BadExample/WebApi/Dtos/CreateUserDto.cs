using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos {
    public class CreateUserDto {
        
        [Required]
        public string Username { get; set; }
    }
}