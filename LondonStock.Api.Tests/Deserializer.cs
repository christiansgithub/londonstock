using System.Text.Json;

namespace LondonStock.Api.Tests;

public static class Deserializer
{
    private static readonly JsonSerializerOptions? Options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<T?> DeserializeAsync<T>(this HttpContent httpContent)
        => JsonSerializer.Deserialize<T>(await httpContent.ReadAsStringAsync(), Options);
}
