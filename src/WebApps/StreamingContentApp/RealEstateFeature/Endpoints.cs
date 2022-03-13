using System.IO.Pipelines;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc;
using StreamingContentApp.Abstractions;

namespace StreamingContentApp.RealEstateFeature; 


/// <summary>
/// This controller demonstrates how to stream json data to the client by manually writing json to the response
/// body and flushing to the stream.
/// </summary>
[ApiController]
[Route("api/realestate")]
public class Endpoints : ControllerBase {
    private readonly IRealEstateRepository repository;

    public Endpoints(IRealEstateRepository repository) {
        this.repository = repository;
    }

    [HttpGet("all-properties")]
    public async void GetAllProperties() {
        IAsyncEnumerable<RealEstate> properties = repository.GetAllAsync();

        HttpContext.Response.ContentType = "application/json; charset=utf-8";
        PipeWriter responseBodyWriter = HttpContext.Response.BodyWriter;
        await responseBodyWriter.WriteAsync(new ReadOnlyMemory<byte>(new byte[] {(byte)'a'}));
        await responseBodyWriter.FlushAsync();

        var jsonSerializerOptions = new JsonSerializerOptions {Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)};

        var jsonWriter = new Utf8JsonWriter(HttpContext.Response.BodyWriter.AsStream());
        jsonWriter.WriteStartArray();
        await foreach (RealEstate realEstate in repository.GetAllAsync()) {
            string json = JsonSerializer.Serialize(
                realEstate,
                realEstate.GetType(), // We pass in the type to enable polymorphic serialization
                jsonSerializerOptions);
            
            jsonWriter.WriteRawValue(json);
            await jsonWriter.FlushAsync();
        }
        jsonWriter.WriteEndArray();
        await jsonWriter.FlushAsync();

        await responseBodyWriter.CompleteAsync();
    }
}