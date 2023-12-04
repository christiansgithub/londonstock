using LondonStock.Data;

namespace LondonStock.Api.Trades;

public class TradeReceiver
{
    private readonly LondonStockContext _db;

    public TradeReceiver(LondonStockContext db)
    {
        _db = db;
    }

    public Task ReceiveTrade(TradeDto trade)
    {
        _db.Trades.Add(new Trade
        {
            StockTicker = trade.StockTicker!.ToUpper(),
            Price = trade.Price,
            NumberOfShares = trade.NumberOfShares,
            BrokerId = trade.BrokerId!,
            CreatedDate = DateTime.UtcNow
        });

        return _db.SaveChangesAsync();
    }
}
