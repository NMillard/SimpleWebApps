using System.Text.Json.Serialization;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Modern.Api.Controllers;

/**
 * 
 */

[ApiController]
[Route("[controller]")]
public class StreamingController : ControllerBase
{
    public StreamingController(BlobServiceClient client, ILogger<StreamingController> logger)
    {
        Client = client;
        this.logger = logger;
    }
    
    private BlobServiceClient Client { get; }
    
    private readonly ILogger<StreamingController> logger;


    /// <remarks>
    /// Calling curl with output flag saves the content from the stream.<br/>
    /// curl -O http://loclhost:5243/streaming/simple-characters
    /// </remarks>
    [HttpGet("simple-file-stream", Name = nameof(SimpleFileStream))]
    public FileResult SimpleFileStream()
    {
        byte[] bytes = "hello there"u8.ToArray();
        var memoryStream = new MemoryStream(bytes);

        return new FileStreamResult(memoryStream, new MediaTypeHeaderValue("application/text"));
    }

    /// <remarks>
    /// Calling curl with no buffer shows each character written to the network stream.<br/>
    /// curl --no-buffer -v http://loclhost:5243/streaming/simple-characters
    /// </remarks>
    [HttpGet("simple-characters", Name = nameof(SimpleCharacterPipe))]
    public async Task<EmptyHttpResult> SimpleCharacterPipe()
    {
        byte[] bytes = "hello there"u8.ToArray();

        foreach (byte character in bytes)
        {
            var text = Convert.ToChar(character).ToString();
            await HttpContext.Response.WriteAsync(text);
            logger.LogInformation("Writing {Text} to stream", text);
        }

        return EmptyHttpResult.Instance;
    }

    /// <summary>
    /// Data is streamed over the network as the buffer is emptied. Meaning, the stream doesn't write object by object
    /// but rather batches a bunch and then writes the batch to the stream.
    /// </summary>
    /// <example>
    /// You can set the buffer size with the global Json Options in Program.cs:
    /// <code>
    /// builder.Services.AddControllers().AddJsonOptions(options => {
    ///     options.JsonSerializerOptions.DefaultBufferSize = 100;
    ///     options.JsonSerializerOptions.WriteIndented = true;
    /// })
    /// </code>
    /// </example>
    /// <returns></returns>
    [HttpGet("json-data")]
    public IAsyncEnumerable<RealEstateBase> JsonData()
    {
        IAsyncEnumerable<RealEstateBase> data = GetData();
        return data;
    }

    /// <summary>
    /// Vastly different memory footprint than the "JsonData" counterpart.
    /// Lots of objects are allocated to the LOH when running this.
    /// </summary>
    [HttpGet("json-data2")]
    public Results<Ok<List<RealEstateBase>>, NotFound> JsonData2()
    {
        List<RealEstateBase> realEstates = GetListData();
        return TypedResults.Ok(realEstates);
    }

    [HttpGet("image-stream")]
    public async Task<IResult> ImageStream()
    {
        var file = new FileStream(
            path: "Images/some-image.png",
            mode: FileMode.Open
        );

        return Results.File(file);
    }
    
    [HttpGet("image-stream-blobstorage")]
    public async Task<IResult> ImageStreamBlobStorage(string fileName)
    {
        BlobContainerClient? containerClient = Client.GetBlobContainerClient("images");
        BlobClient? blob = containerClient.GetBlobClient(fileName);

        bool unknownImage = !await blob.ExistsAsync();
        if (unknownImage) return Results.BadRequest(new ProblemDetails
        {
            Detail = "The requested image doesn't exist"
        });
        
        Response<BlobDownloadStreamingResult>? download = await blob.DownloadStreamingAsync();
        if (!download.HasValue) return Results.BadRequest(new ProblemDetails());
        
        Stream stream = download.Value.Content;
        HttpContext.Response.Headers.ContentLength = download.Value.Details.ContentLength;
        return Results.File(stream, contentType: download.Value.Details.ContentType);
    }

    [HttpGet("video-stream")]
    public IResult VideoStream()
    {
        var file = new FileStream(
            path: "Images/recording.mp4",
            mode: FileMode.Open);

        return Results.File(
            fileStream: file,
            enableRangeProcessing: true);
    }

    [HttpGet("download-blob-file")]
    public async Task<IResult> DownloadBlobFile(string fileName)
    {
        BlobContainerClient? containerClient = Client.GetBlobContainerClient("images");
        BlobClient? blob = containerClient.GetBlobClient(fileName);

        bool unknownImage = !await blob.ExistsAsync();
        if (unknownImage) return Results.BadRequest(new ProblemDetails
        {
            Detail = "The requested image doesn't exist"
        });

        Response<BlobDownloadStreamingResult>? download = await blob.DownloadStreamingAsync();
        if (!download.HasValue) return Results.BadRequest(new ProblemDetails());
        
        Stream stream = download.Value.Content;
        HttpContext.Response.Headers.ContentLength = download.Value.Details.ContentLength;
        return Results.File(stream, contentType: download.Value.Details.ContentType, fileDownloadName: fileName);
    }

    private List<RealEstateBase> GetListData()
    {
        var condominium = new Condominium
        {
            CadastralId = Guid.NewGuid(),
            HomeownersAssociation = "Some association",
            MonthlyHousingFee = 1000
        };
        return new List<RealEstateBase>(Enumerable.Repeat(condominium, 1_000_000));
    }

    private async IAsyncEnumerable<RealEstateBase> GetData()
    {
        var condominium = new Condominium
        {
            CadastralId = Guid.NewGuid(),
            HomeownersAssociation = "Some association",
            MonthlyHousingFee = 1000
        };

        var range = Enumerable.Repeat(condominium, 1_000_000);
        foreach (RealEstateBase realEstateBase in range)
        {
            if (HttpContext.RequestAborted.IsCancellationRequested)
            {
                logger.LogInformation("Request aborted");
                break;
            }

            yield return realEstateBase;
        }
    }
}

[JsonDerivedType(typeof(Condominium), typeDiscriminator: nameof(Condominium))]
[JsonDerivedType(typeof(Townhouse), typeDiscriminator: nameof(Townhouse))]
[JsonDerivedType(typeof(BuildingOnLeasedLand), typeDiscriminator: nameof(BuildingOnLeasedLand))]
public abstract class RealEstateBase
{
    public Guid CadastralId { get; set; }
}

public class Condominium : RealEstateBase
{
    public string HomeownersAssociation { get; set; }
    public decimal MonthlyHousingFee { get; set; }
}

public class BuildingOnLeasedLand : RealEstateBase
{
    public RentalPeriod RentalPeriod { get; set; }
}

public class Townhouse : RealEstateBase
{
    public int HousesInRowCount { get; set; }
}

public record RentalPeriod
{
    public required DateOnly RentalStartDate { get; init; }
    public required DateOnly RentalEndDate { get; init; }
}