# Parsing Huge Json Files using Streams for Efficient Memory Usage
## It's easier than you'd think (at least it is in .NET)

Have you ever had to parse a huge JSON file that was too large to keep in memory? Or possibly even a zipped file, that'd be too large to extract on the server. Perhaps even something radical as streaming the zipped JSON over SFTP and parsing the incoming zip stream on the fly?  

Probably not. But it's an exciting issue. One that I've recently had to deal with.  

On a client project, we've had to download a `2GB` zip archive that contains a `60GB` JSON file. Now, this is obviously too big to extract on the server.  

So, instead, we parsed the incoming zip stream on the fly to keep memory usage low.  

I typically don't write tutorials, as most of you have probably already noticed. But I'll make an exception for this case due to its interesting feats.  

The actual client project is in Java, but I've translated parts of our solution to .NET as that's my preferred platform.

### Our solution was honestly surprisingly simple.
Once you have a zip stream and the zip entry--containing a JSON file in this case--you can act on it as if it was a regular file stream containing json data. To me, that was quite surprising.  

Before we get ahead of ourselves, let's back up a bit.  

To those who are new to streams and in particular JSON streams, we'll take a few seconds to comprehend how we usually go about handling streams with JSON data.  

Note that I'm not covering the simple case of deserializing a json string into a typed object. This is too trivial and you can find tons of great examples of how to do this already.

Anyway, say we have the following JSON in a regular `.json` file.  

```json5
// data.json file
[
  {
    "id": 1,
    "firstName": "Thomas",
    "lastName": "Bækker",
    "address": {
      "streetName": "Tornelisevej",
      "streetNumber": "10",
      "city": "Solrød Strand"
    },
    "emergencyPhoneNumbers": ["+4530303030", "+4510203040"]
  },
  {
    "id": 2,
    "firstName": "Niels",
    "lastName": "Bohr",
    "address": {
      "streetName": "Flemsevej",
      "streetNumber": "31A",
      "city": "København"
    },
    "emergencyPhoneNumbers": []
  },
  ...
]
```

When you want to read this, as a stream, you just open the file to get a `FileStream` and then pass this stream to `JsonDocument`, or a `JsonNode` and use their respective `Parse(stream)` methods.

````csharp
using FileStream file = File.OpenRead("files/data.json");

JsonElement json = JsonDocument.Parse(file).RootElement;
if (json.ValueKind != JsonValueKind.Array) return;

foreach (JsonElement element in json.EnumerateArray()) {
    int id = element.GetProperty("id").GetInt32();
    testOutputHelper.WriteLine(id.ToString());
}
````
When working with streams of data, you need to validate your assumptions before performing any actual parsing, hence checking if the current element is an `Array`.

This will work well with small amounts of json data, but the application will crash with huge datasets. `JsonDocument.Parse(stream)` reads the entire stream to completion upon invocation, which defeats the purpose of a stream, really...  

Reading the entire stream may cause an `OverflowException` when dealing with a humongous dataset, filled with complex objects. 





## More resources for the curious
- https://github.com/dotnet/designs/blob/main/accepted/2020/serializer/WriteableDomAndDynamic.md#interop-with-jsonelement