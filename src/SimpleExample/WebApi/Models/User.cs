using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

/*
 * User class that acts as domain, persistence and transfer model.
 * Way too many responsibilities crammed into one class.
 */

namespace WebApi.Models {
    
    [Table("Users")]
    public class User : EntityBase {

        public User() {
            Articles = new List<Article>();
        }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [JsonIgnore]
        public byte[] ProfileImage { get; set; }

        public List<Article> Articles { get; set; }
    }
}