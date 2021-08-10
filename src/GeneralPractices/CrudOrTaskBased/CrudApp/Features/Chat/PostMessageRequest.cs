using System;
using System.ComponentModel.DataAnnotations;

namespace CrudApp.Features.Chat {
    public class PostMessageRequest {
        [Required]
        public Guid ChannelId { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }
    }
}