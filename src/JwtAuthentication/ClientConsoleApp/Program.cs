using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

var httpClient = new HttpClient {BaseAddress = new Uri("https://localhost:5001")};

var token = await httpClient.GetFromJsonAsync<JwtToken>("api/test/generate");
Console.WriteLine(token?.Token);

httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Token);
HttpResponseMessage result = await httpClient.GetAsync("api/test");

Console.WriteLine();

public class JwtToken {
    public string Token { get; set; }
}

