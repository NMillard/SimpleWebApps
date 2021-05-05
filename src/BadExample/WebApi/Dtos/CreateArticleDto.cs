using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos {
    public class CreateArticleDto {
        [Required]
        public string Content { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}