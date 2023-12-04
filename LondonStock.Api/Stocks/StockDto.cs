namespace LondonStock.Api.Stocks;

public class StockDto
{
    public required string StockTicker { get; init; }

    public decimal Price { get; init; }
}
