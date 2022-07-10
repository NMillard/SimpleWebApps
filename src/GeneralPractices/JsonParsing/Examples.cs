using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using Xunit;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonParsing;

public class UnitTest1 {
    private readonly ITestOutputHelper testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper) {
        this.testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// While JsonDocument works fine for smaller json files, it doesn't quite work well on large files since the whole
    /// document is read until completion.
    /// </summary>
    [Fact]
    public void RegularJsonStreamParsingWithoutType() {
        using FileStream file = File.OpenRead("files/data.json");

        JsonElement json = JsonDocument.Parse(file).RootElement;
        if (json.ValueKind != JsonValueKind.Array) return;

        foreach (JsonElement element in json.EnumerateArray()) {
            int id = element.GetProperty("id").GetInt32();
            testOutputHelper.WriteLine(id.ToString());
        }
    }

    [Fact]
    public void RegularJsonStreamParsingWithType() {
        using FileStream file = File.OpenRead("files/data.json");

        JsonElement json = JsonDocument.Parse(file).RootElement;
        if (json.ValueKind != JsonValueKind.Array) return;

        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        foreach (JsonElement element in json.EnumerateArray()) {
            var employee = element.Deserialize<Employee>(jsonSerializerOptions);
            testOutputHelper.WriteLine(employee?.Id.ToString());
        }
    }

    [Fact]
    public async Task ParsingJsonStreamOneElementAtTheTimeNoDep() {
        await using FileStream file = File.OpenRead("files/data.json");
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        IAsyncEnumerable<JsonNode?> enumerable = JsonSerializer.DeserializeAsyncEnumerable<JsonNode>(file, options);
        await foreach (JsonNode? obj in enumerable) {
            var id = obj?["id"]?.GetValue<int>();
            testOutputHelper.WriteLine($"{id}");
        }
    }

    /// <summary>
    /// Reading a JSON stream using Newtonsoft instead of System.Text.Json
    /// </summary>
    [Fact]
    public async Task ParsingJsonStreamOneElementAtTheTimeWithNewtonsoft() {
        await using FileStream file = File.OpenRead("files/data.json");

        using var streamReader = new StreamReader(file);
        using var jsonReader = new JsonTextReader(streamReader);

        while (jsonReader.Read()) {
            if (jsonReader.TokenType != JsonToken.StartObject) continue;

            var obj = new Newtonsoft.Json.JsonSerializer().Deserialize<JObject>(jsonReader);
            var e = obj["id"].ToString();
        }
    }
    
    [Fact]
    public async Task ParseJsonFromZippedFile() {
        await using FileStream file = File.OpenRead("files/data.zip");
        using var zipArchive = new ZipArchive(file);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries) {
            await using Stream entryStream = zipEntry.Open();

            IAsyncEnumerable<JsonNode?> enumerable = JsonSerializer.DeserializeAsyncEnumerable<JsonNode>(entryStream, options);
            await foreach (JsonNode? obj in enumerable) { 
                // Read single property
                var id = obj?["id"]?.ToString();
                
                // Parse whole object
                var employee = obj.Deserialize<Employee>(options);

                // Parse only subset of the object
                JsonNode? addressNode = obj?["address"];
                if (addressNode is null) continue;
                
                var address = addressNode.Deserialize<Address>(options);
            }
        }
    }

    [Fact]
    public async Task ParseJsonFromZippedFileOverHttp() {
        var client = new HttpClient();
        Stream response = await client.GetStreamAsync("https://localhost:7136/FileStream");

        var zipArchive = new ZipArchive(response);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        foreach (ZipArchiveEntry zipEntry in zipArchive.Entries) {
            await using Stream entryStream = zipEntry.Open();

            IAsyncEnumerable<JsonNode?> enumerable = JsonSerializer.DeserializeAsyncEnumerable<JsonNode>(entryStream, options);
            await foreach (JsonNode? obj in enumerable) { 
                testOutputHelper.WriteLine(obj?["id"]?.ToString());
            }
        }
    }
}

internal class Employee {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Type { get; set; }
    public List<string> EmergencyPhoneNumbers { get; set; } = new();
}

internal class Address {
    public string StreetName { get; set; }
    public string StreetNumber { get; set; }
    public string City { get; set; }
}