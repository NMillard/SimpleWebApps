// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonParsing.Benchmark; 

public static class BenchmarkTests {
    public static void Main(string[] args) {
        BenchmarkRunner.Run<FileStreamer>();
    }
}

[MemoryDiagnoser]
public class FileStreamer {
    public async Task StreamFile() {
        await using FileStream file = File.OpenRead("files/large-file.zip");
        using var zipArchive = new ZipArchive(file);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries) {
            await using Stream entryStream = zipEntry.Open();

            IAsyncEnumerable<JsonNode?> enumerable = JsonSerializer.DeserializeAsyncEnumerable<JsonNode>(entryStream, options);
            await foreach (JsonNode? obj in enumerable) {
                var e = obj["id"].ToString();
            }
        }
    }

    public async Task StreamFileOverHttp() {
        var client = new HttpClient();
        Stream response = await client.GetStreamAsync("https://localhost:7136/FileStream");

        var zipArchive = new ZipArchive(response);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries) {
            await using Stream entryStream = zipEntry.Open();

            IAsyncEnumerable<JsonNode?> enumerable = JsonSerializer.DeserializeAsyncEnumerable<JsonNode>(entryStream, options);
            await foreach (JsonNode? obj in enumerable) {
                var e = obj["id"].ToString();
            }
        }
    }


    [Benchmark]
    public async IAsyncEnumerable<string> StreamFileNewtonsoftAsyncReturn() {
        await using FileStream file = File.OpenRead("files/large-file.zip");
        using var zipArchive = new ZipArchive(file);

        foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries) {
            using var streamReader = new StreamReader(zipArchiveEntry.Open());
            using var jsonReader = new JsonTextReader(streamReader);

            while (jsonReader.Read()) {
                if (jsonReader.TokenType != JsonToken.StartObject) continue;

                var obj = new Newtonsoft.Json.JsonSerializer().Deserialize<JObject>(jsonReader);
                yield return obj["id"].ToString();
            }
        }
    }
    
    [Benchmark]
    public async Task StreamFileNewtonsoft() {
        await using FileStream file = File.OpenRead("files/large-file.zip");
        using var zipArchive = new ZipArchive(file);

        foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries) {
            using var streamReader = new StreamReader(zipArchiveEntry.Open());
            using var jsonReader = new JsonTextReader(streamReader);

            while (jsonReader.Read()) {
                if (jsonReader.TokenType != JsonToken.StartObject) continue;

                var obj = new Newtonsoft.Json.JsonSerializer().Deserialize<JObject>(jsonReader);
                obj["id"].ToString();
            }
        }
    }
}