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

        public FilesController(
            RemovingTraditionalBranching.Traditional.FileSaver fileSaverTraditional,
            RemovingTraditionalBranching.Branchless.FileSaver fileSaverBranchless) {
            this.fileSaverTraditional = fileSaverTraditional;
            this.fileSaverBranchless = fileSaverBranchless;
        }
        
        [HttpPost("SaveFile/Branchless/FileSystem")]
        public async Task<IActionResult> BranchlessFileSystemSaveFile(IFormFile file) {
            byte[] content = Encoding.UTF8.GetBytes(await new StreamReader(file.OpenReadStream()).ReadToEndAsync());
            var fileName = $"{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}";
            await fileSaverBranchless.StoreFile(new FileSystemContext(content, fileName, "branchless"));
            
            return Ok();
        }
        
        [HttpPost("SaveFile/Branchless/Database")]
        public async Task<IActionResult> BranchlessDatabaseSaveFile(IFormFile file) {
            byte[] content = Encoding.UTF8.GetBytes(await new StreamReader(file.OpenReadStream()).ReadToEndAsync());
            var fileName = $"{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}";
            await fileSaverBranchless.StoreFile(new DatabaseFileContext(content, fileName));
            
            return Ok();
        }
    }
}