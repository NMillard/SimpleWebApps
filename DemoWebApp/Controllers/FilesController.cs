using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemovingTraditionalBranching.Branchless;

namespace DemoWebApp.Controllers {
    
    [Route("api/[controller]")]
    public class FilesController : ControllerBase {
        private readonly RemovingTraditionalBranching.Traditional.FileSaver fileSaverTraditional;
        private readonly RemovingTraditionalBranching.Branchless.FileSaver fileSaverBranchless;
        private readonly IEnumerable<IFileSink> sinks;

        public FilesController(
            RemovingTraditionalBranching.Traditional.FileSaver fileSaverTraditional,
            RemovingTraditionalBranching.Branchless.FileSaver fileSaverBranchless,
            IEnumerable<IFileSink> sinks) {
            this.fileSaverTraditional = fileSaverTraditional;
            this.fileSaverBranchless = fileSaverBranchless;
            this.sinks = sinks;
        }
        
        [HttpPost("SaveFile/Extensions")]
        public async Task<IActionResult> TraditionalSaveFile(IFormFile file) {
            byte[] content = Encoding.UTF8.GetBytes(await new StreamReader(file.OpenReadStream()).ReadToEndAsync());
            await sinks.SaveFile(new FileSystemContext(content, $"{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}", "traditional"));
            return Ok();
        }
    }
}