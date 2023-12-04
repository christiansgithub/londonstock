using System.Net;
using FluentAssertions;
using LondonStock.Api.Stocks;
using LondonStock.Api.Tests.Extensions;
using LondonStock.Api.Trades;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LondonStock.Api.Tests;

public class StocksTests : IClassFixture<WebApplicationFactoryWithInMemoryDb>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StocksTests(WebApplicationFactoryWithInMemoryDb factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTrade_WithWhitespace_ShouldReturnBadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/stocks/%20");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTrade_WithStockTickerThatDoesNotExist_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/stocks/INVALID_TICKER");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTrade_WithStockTickerThatExists_ShouldReturn()
    {
        var client = _factory.CreateClient();
        await client.CreateTrade(new TradeDto
        {
            StockTicker = "ABC",
            NumberOfShares = 1,
            BrokerId = "B1",
            Price = 10
        });

        var response = await client.GetAsync("/stocks/ABC");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Assumption that ticker ids should to case insensitive, but requirement would need clarifying
    [Fact]
    public async Task GetTrade_WithStockTickerThatIsMixedCase_ShouldReturn()
    {
        var client = _factory.CreateClient();
        await client.CreateTrade(new TradeDto
        {
            StockTicker = "aBc",
            NumberOfShares = 1,
            BrokerId = "B1",
            Price = 10
        });

        var response = await client.GetAsync("/stocks/AbC");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Requirements assumption is that the current stock price is the most recently received trade price
    [Fact]
    public async Task GetTrade_WithStockTickerThatExistsWithMultipleTrades_ShouldReturnMostRecentInformation()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.CreateTrade(new TradeDto { StockTicker = "ABC", NumberOfShares = 1, BrokerId = "B1", Price = 10 });
        await client.CreateTrade(new TradeDto { StockTicker = "ABC", NumberOfShares = 2, BrokerId = "B1", Price = 12 });
        await client.CreateTrade(new TradeDto { StockTicker = "ABC", NumberOfShares = 1, BrokerId = "B1", Price = 6 });

        // Act
        var response = await client.GetAsync("/stocks/ABC");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.DeserializeAsync<StockDto>();

        result.Should().BeEquivalentTo(new StockDto
        {
            StockTicker = "ABC",
            Price = 6 // Latest trade price
        });
    }

    [Fact]
    public async Task GetAllStocks_WhenMultipleTrades_ShouldReturnFullList()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.CreateTrade(new TradeDto { StockTicker = "ULVR", NumberOfShares = 1, BrokerId = "B1", Price = 10 });
        await client.CreateTrade(new TradeDto { StockTicker = "NXT", NumberOfShares = 2, BrokerId = "B1", Price = 12 });
        await client.CreateTrade(new TradeDto { StockTicker = "AZN", NumberOfShares = 1, BrokerId = "B1", Price = 6 });
        await client.CreateTrade(new TradeDto { StockTicker = "AZN", NumberOfShares = 1, BrokerId = "B1", Price = 5 });

        // Act
        var response = await client.GetAsync("/stocks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.DeserializeAsync<DataWrapper<StockDto>>();

        result.Should().BeEquivalentTo(new DataWrapper<StockDto>
        {
            Data = new[]
            {
                new StockDto
                {
                    StockTicker = "ULVR",
                    Price = 10
                },
                new StockDto
                {
                    StockTicker = "NXT",
                    Price = 12
                },
                new StockDto
                {
                    StockTicker = "AZN",
                    Price = 5
                }
            }
        });
    }

    [Fact]
    public async Task SearchStocks_WhenMultipleTrades_ShouldReturnForGivenTickers()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.CreateTrade(new TradeDto { StockTicker = "ULVR", NumberOfShares = 1, BrokerId = "B1", Price = 10 });
        await client.CreateTrade(new TradeDto { StockTicker = "NXT", NumberOfShares = 2, BrokerId = "B1", Price = 12 });
        await client.CreateTrade(new TradeDto { StockTicker = "AZN", NumberOfShares = 1, BrokerId = "B1", Price = 6 });
        await client.CreateTrade(new TradeDto { StockTicker = "AZN", NumberOfShares = 1, BrokerId = "B1", Price = 5 });

        // Act
        var response = await client.GetAsync("/stocks/search?tickers=ULVR,AZN");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.DeserializeAsync<DataWrapper<StockDto>>();

        result.Should().BeEquivalentTo(new DataWrapper<StockDto>
        {
            Data = new[]
            {
                new StockDto
                {
                    StockTicker = "ULVR",
                    Price = 10
                },
                new StockDto
                {
                    StockTicker = "AZN",
                    Price = 5
                }
            }
        });
    }

    [Fact]
    public async Task SearchStocks_WhenNoTickersGivenInSearch_ShouldReturnBadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/stocks/search");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
