using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamZipFile.WebApi.Controllers;

[ApiController]
[Route("api/processes")]
public class ProcessController : ControllerBase
{

    private readonly ProcessorChannel channel;

    public ProcessController(ProcessorChannel channel)
    {
        this.channel = channel;
    }

    [HttpGet("run")]
    public async Task<ActionResult<dynamic>> Run(CancellationToken token)
    {
        StreamReader file = System.IO.File.OpenText("TestFiles/demo.json");
        var reader = new JsonTextReader(file);
        
        var serializer = new JsonSerializer();
        while (await reader.ReadAsync(token))
        {
            while (reader.TokenType != JsonToken.StartArray) await reader.ReadAsync(token);

            var array = serializer.Deserialize<JArray>(reader);
            foreach (JToken jToken in array)
            {
                await channel.AddElementAsync(jToken.ToString(), token);
            }
        }
        
        return Ok();
    }
}