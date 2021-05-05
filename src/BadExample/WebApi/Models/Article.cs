using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApi.Models {
    public class Article : EntityBase {
        
        [JsonIgnore]
        public User User { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType("DateTime2")]
        public DateTime TimePublished { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
    }
}