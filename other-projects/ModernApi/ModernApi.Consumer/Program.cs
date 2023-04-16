using System.Text.Json;
using System.Text.Json.Nodes;

Console.WriteLine("Start consuming API");

var client = new HttpClient
{
    BaseAddress = new UriBuilder("http://localhost:5243").Uri
};

Console.WriteLine("Start");
// string s = await client.GetStringAsync("/streaming/json-data2");
// List<JsonNode>? l = JsonSerializer.Deserialize<List<JsonNode>>(s)?.ToList();

// Console.WriteLine($"Received: {l?.Count}");

await using Stream stream =  await client.GetStreamAsync("/streaming/json-data");
var count = 0;
IAsyncEnumerable<JsonNode?> e = JsonSerializer.DeserializeAsyncEnumerable<JsonNode>(stream);
await foreach (JsonNode jsonNode in e)
{
    if (jsonNode is null) continue;
    string jsonString = jsonNode.ToJsonString();
    count++;
}

Console.WriteLine($"Received: {count}");

// var reader = new StreamReader(stream);
//
// while (!reader.EndOfStream)
// {
//     string? s = reader.ReadLine();
//     Console.WriteLine(s);
// }

Console.WriteLine("Done");


