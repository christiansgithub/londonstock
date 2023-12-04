using Microsoft.EntityFrameworkCore;

namespace LondonStock.Data;

[Index(nameof(StockTicker), nameof(CreatedDate))]
public class Trade
{
    public int Id { get; set; }

    public required string StockTicker { get; set; }

    public decimal Price { get; set; }

    public decimal NumberOfShares { get; set; }

    public required string BrokerId { get; set; }

    public DateTime CreatedDate { get; set; }
}
