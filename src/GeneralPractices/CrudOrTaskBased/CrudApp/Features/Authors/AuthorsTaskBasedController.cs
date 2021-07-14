using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CrudApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Features.Authors {
    [Route("api/authors")]
    public class AuthorsTaskBasedController : ControllerBase {
        private readonly AuthorRepository repository;

        public AuthorsTaskBasedController(AuthorRepository repository) => this.repository = repository;

        [HttpPatch("{id:guid}/ChangeEmail")]
        public async Task<ActionResult> ChangeEmail(Guid id, ChangeEmailModel model)
            => await UpdateAuthorProperty(id, author => author.Email = model.Email);

        [HttpPatch("{id:guid}/UpdateBio")]
        public async Task<ActionResult> UpdateBio(Guid id, UpdateBioModel model)
            => await UpdateAuthorProperty(id, author => author.Bio = model.Bio);

        [HttpPatch("{id:guid}/ChangeRealName")]
        public async Task<ActionResult> ChangeRealName(Guid id, ChangeRealName model)
            => await UpdateAuthorProperty(id, author => author.RealName = model.RealName);

        [HttpPatch("{id:guid}/AddPenName/{penName:alpha}")]
        public async Task<ActionResult> AddPenName(Guid id, string penName)
            => await UpdateAuthorProperty(id, author => author.PenNames.Add(penName));

        [HttpDelete("{id:guid}/RemovePenName/{penName:alpha}")]
        public async Task<ActionResult> RemovePenName(Guid id, string penName)
            => await UpdateAuthorProperty(id, author => author.PenNames.Remove(penName));

        private async Task<ActionResult> UpdateAuthorProperty(Guid id, Action<Author> action) {
            if (await repository.GetAsync(id) is not { } author) return BadRequest();

            action(author);

            bool result = await repository.UpdateAsync(author);
            return result ? Ok() : BadRequest();
        }
    }

    public record ChangeEmailModel {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }

    public record UpdateBioModel {
        [MaxLength(500)] public string Bio { get; set; }
    }

    public record ChangeRealName {
        [MaxLength(150)] public string RealName { get; set; }
    }
}