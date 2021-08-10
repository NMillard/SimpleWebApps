using System;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Features.Chat {
    
    [Route("api/chat")]
    public class ChatTaskBasedController : ControllerBase {


        [HttpGet("Channel/{id:guid}/[action]")]
        public IActionResult ViewHistory(Guid id) {
            return Ok();
        }
        
        [HttpPost("[action]")]
        public IActionResult StartNewChannel() {
            return Ok();
        }

        [HttpPatch("Channel/{id:guid}/[action]")]
        public IActionResult ChangeName(Guid id) {
            return Ok();
        }

        [HttpPost("Channel/{id:guid}/[action]")]
        public IActionResult PublishMessage(Guid id) {
            return Ok();
        }
        
        
        [HttpPost("Message/{id:guid}/[action]")]
        public IActionResult Reply(Guid id) {
            return Ok();
        }
        
        [HttpGet("Message/{id:guid}/[action]")]
        public IActionResult ShowReplies(Guid id) {
            return Ok();
        }

    }
}