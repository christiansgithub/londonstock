using LondonStock.Data;
using Microsoft.EntityFrameworkCore;

namespace LondonStock.Api.Stocks;

public class StockInformation
{
    private readonly LondonStockContext _db;

    public StockInformation(LondonStockContext db)
    {
        _db = db;
    }

    public async Task<StockDto?> ForStockTicker(string stockTicker)
    {
        var latestTrade = await _db
            .Trades
            .OrderByDescending(x => x.CreatedDate)
            .Where(x => x.StockTicker == stockTicker.ToUpper())
            .FirstOrDefaultAsync();

        if (latestTrade is null)
        {
            return null;
        }

        return new StockDto
        {
            StockTicker = latestTrade.StockTicker,
            Price = latestTrade.Price
        };
    }

    public async Task<StockDto[]> ForAllStockTickers()
    {
        return await _db
            .Trades
            .GroupBy(x => x.StockTicker)
            .Select(g => new StockDto
            {
                StockTicker = g.Key,
                Price = g.OrderByDescending(e => e.CreatedDate).First().Price
            })
            .ToArrayAsync();
    }

    public async Task<StockDto[]> ForStockTickers(IEnumerable<string> tickers)
    {
        var tickersUpperCase = tickers.Select(x => x.ToUpper()).ToArray();

        return await _db
            .Trades
            .Where(x => tickersUpperCase.Contains(x.StockTicker))
            .GroupBy(x => x.StockTicker)
            .Select(g => new StockDto
            {
                StockTicker = g.Key,
                Price = g.OrderByDescending(e => e.CreatedDate).First().Price
            })
            .ToArrayAsync();
    }
}
