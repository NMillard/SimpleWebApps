using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Features.Authors {
    
    [Route("api/[controller]")]
    public class AuthorsCrudController : ControllerBase {
        private readonly AuthorRepository repository;
        public AuthorsCrudController(AuthorRepository repository) => this.repository = repository;

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, UpdateAuthorModel model) {
            if (await repository.GetAsync(id) is not { } author) return BadRequest();

            author.PenNames = model.PenNames.ToList();
            author.RealName = model.RealName;
            author.Bio = model.Bio;
            author.Email = model.Email;

            bool result = await repository.UpdateAsync(author);

            return result ? Ok() : BadRequest();
        }

        [HttpPatch("v2/{id:guid}")]
        public async Task<ActionResult> UpdateV2(Guid id, UpdateAuthorConditionally model) {
            if (await repository.GetAsync(id) is not { } author) return BadRequest();

            switch (model.UpdateReason) {
                case UpdateReason.AddedPenName:
                    author.PenNames.Add(model.PenName);
                    break;
                case UpdateReason.RemovedPenName:
                    author.PenNames.Remove(model.PenName);
                    break;
                case UpdateReason.ChangedEmail:
                    author.Email = model.Email;
                    // Publish message to service bus
                    break;
                case UpdateReason.UpdatedBio:
                    author.Bio = model.Bio;
                    break;
                case UpdateReason.UpdatedRealName:
                    author.RealName = model.RealName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(model));
            }

            bool result = await repository.UpdateAsync(author);
            
            return result ? Ok() : BadRequest();
        }
    }

    public class UpdateAuthorConditionally {
        public string PenName { get; set; }
        public string RealName { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public UpdateReason UpdateReason { get; set; }
    }

    public enum UpdateReason {
        AddedPenName,
        RemovedPenName,
        ChangedEmail,
        UpdatedBio,
        UpdatedRealName
    }

    public record UpdateAuthorModel {
        public string[] PenNames { get; set; }
        public string RealName { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
    }
}