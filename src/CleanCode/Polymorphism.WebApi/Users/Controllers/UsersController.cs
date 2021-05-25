using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polymorphism.WebApi.Users.Commands;
using Polymorphism.WebApi.Users.Queries;

namespace Polymorphism.WebApi.Users.Controllers {
    
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private readonly ICreateUserCommand command;
        private readonly IGetAllUsersQuery query;

        public UsersController(ICreateUserCommand command, IGetAllUsersQuery query) {
            this.command = command;
            this.query = query;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await query.ExecuteAsync());
        

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserInput model) {
            await command.ExecuteAsync(model);
            return Ok();
        }

        #region polymorphic
        [HttpPost("create/regular")]
        public async Task<IActionResult> CreateRegularUser(CreateRegularUserInput model) {
            await command.ExecuteAsync(model);
            
            return Ok();
        }
        
        [HttpPost("create/premium")]
        public async Task<IActionResult> CreatePremiumUser(CreatePremiumUserInput model) {
            await command.ExecuteAsync(model);
            
            return Ok();
        }
        #endregion
    }
}