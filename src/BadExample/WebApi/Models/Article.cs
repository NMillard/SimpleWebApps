using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApi.Models {
    public class Article : EntityBase, ICsvSerializable {
        
        [JsonIgnore]
        public User User { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType("DateTime2")]
        public DateTime TimePublished { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public string[] GetCsvPropertyNames() {
            return new[] {
                nameof(UserId),
                nameof(TimePublished)
            };
        }

        public string ToCsv() {
            return $"{UserId.ToString()},{TimePublished:d}";
        }
    }
}