using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using LondonStock.Api.Trades;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LondonStock.Api.Tests;

public class TradesTests : IClassFixture<WebApplicationFactoryWithInMemoryDb>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TradesTests(WebApplicationFactoryWithInMemoryDb factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateNewTrade_WhenPostingValidTrade_ShouldReturnSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var trade = new TradeDto
        {
            StockTicker = "AZN",
            Price = 10_000,
            NumberOfShares = 10,
            BrokerId = "B1"
        };

        // Act
        var response = await client.PostAsync("/trades", new StringContent(JsonSerializer.Serialize(trade), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateNewTrade_WhenPostingTradeWithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var trade = new
        {
        };

        // Act
        var response = await client.PostAsync("/trades", new StringContent(System.Text.Json.JsonSerializer.Serialize(trade), Encoding.UTF8, "application/json")); ;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var validation = await response.Content.DeserializeAsync<ExpectedValidationResult>();

        validation.Should().NotBeNull();
        validation!.Errors.Should()
            .SatisfyRespectively(
                first =>
                {
                    first.Key.Should().Be(nameof(TradeDto.Price));
                    first.Value.Should().BeEquivalentTo(new[] { "'Price' must be greater than '0'." });
                },
                second =>
                {
                    second.Key.Should().Be(nameof(TradeDto.BrokerId));
                    second.Value.Should().BeEquivalentTo(new[] { "'Broker Id' must not be empty." });
                },
                third =>
                {
                    third.Key.Should().Be(nameof(TradeDto.StockTicker));
                    third.Value.Should().BeEquivalentTo(new[] { "'Stock Ticker' must not be empty." });
                },
                fourth =>
                {
                    fourth.Key.Should().Be(nameof(TradeDto.NumberOfShares));
                    fourth.Value.Should().BeEquivalentTo(new[] { "'Number Of Shares' must be greater than '0'." });
                });
    }
}

internal record ExpectedValidationResult(Dictionary<string, string[]> Errors);
