using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace StreamZipFile.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FileStreamController : ControllerBase {
    
    [HttpGet(Name = "Get")]
    public FileStreamResult Get() {
        FileStream e = System.IO.File.OpenRead("files/large-file.zip");
        return File(e, "application/zip");
    }
}