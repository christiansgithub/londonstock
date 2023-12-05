using Microsoft.AspNetCore.Mvc;

namespace LondonStock.Api.Stocks;

[ApiController]
[Route("stocks")]
public class StocksController : ControllerBase
{
    private readonly StockInformation _stockInformation;

    public StocksController(StockInformation stockInformation)
    {
        _stockInformation = stockInformation;
    }

    [HttpGet("{stockTicker}", Name = "GetStock")]
    public async Task<IActionResult> GetCount(string stockTicker)
    {
        if (string.IsNullOrWhiteSpace(stockTicker))
        {
            return BadRequest();
        }

        var stockInformation = await _stockInformation.ForStockTicker(stockTicker);

        if (stockInformation is null)
        {
            return NotFound();
        }

        return Ok(stockInformation);
    }

    [HttpGet(Name = "GetAllStocks")]
    public async Task<IActionResult> GetAllStocks()
    {
        var stockInformation = await _stockInformation
            .ForAllStockTickers();

        return Ok(new DataWrapper<StockDto>
        {
            Data = stockInformation
        });
    }

    // The tickers query parameter is a comma separated list of ticker symbols e.g. ?tickers=AA,BB
    [HttpGet("search", Name = "SearchStocks")]
    public async Task<IActionResult> SearchStocks([FromQuery] string tickers)
    {
        var tickerList = tickers.Split(",");

        var stockInformation = await _stockInformation
            .ForStockTickers(tickerList);

        return Ok(new DataWrapper<StockDto>
        {
            Data = stockInformation
        });
    }
}

// Returning with a Data property for easier future extensibility, for example adding paging information later
public record DataWrapper<T>
{
    public required T[] Data { get; init; }
}
