using Microsoft.AspNetCore.Mvc;
using WebApiSecrets.Configurations;

namespace WebApiSecrets.Controllers {

    [Route("api/[controller]")]
    public class SecretsController : ControllerBase {
        private readonly ConnectionStringOptions options;

        public SecretsController(ConnectionStringOptions options) {
            this.options = options;
        }

        [HttpGet(nameof(ConnectionStrings))]
        public IActionResult ConnectionStrings() => Ok(options);
    }
}