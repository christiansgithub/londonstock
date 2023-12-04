using LondonStock.Data;
using Microsoft.AspNetCore.Mvc;

namespace LondonStock.Api.Trades;

[ApiController]
[Route("trades")]
public class TradesController : ControllerBase
{
    private readonly TradeReceiver _tradeReceiver;
    private readonly LondonStockContext _db;

    public TradesController(TradeReceiver tradeReceiver, LondonStockContext db)
    {
        _tradeReceiver = tradeReceiver;
        _db = db;
    }

    [HttpPost(Name = "CreateNewTrade")]
    public async Task<IActionResult> Post([FromBody] TradeDto trade)
    {
        await _tradeReceiver.ReceiveTrade(trade);

        return NoContent();
    }
}
