using System.Text;
using System.Text.Json;
using LondonStock.Api.Trades;

namespace LondonStock.Api.Tests.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> CreateTrade(this HttpClient client, TradeDto trade)
    {
        var response = await client.PostAsync("/trades",
            new StringContent(JsonSerializer.Serialize(trade), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        return response;
    }
}
