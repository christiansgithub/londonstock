using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace LondonStock.Api.Trades;

public record TradeDto
{
    public string? StockTicker { get; init; }

    public decimal Price { get; init; }

    public decimal NumberOfShares { get; init; }

    public string? BrokerId { get; init; }
}

public class TradeDtoValidator : AbstractValidator<TradeDto>
{
    public TradeDtoValidator()
    {
        RuleFor(trade => trade.StockTicker).NotNull();
        RuleFor(trade => trade.Price).GreaterThan(0);
        RuleFor(trade => trade.NumberOfShares).GreaterThan(0);
        RuleFor(trade => trade.BrokerId).NotNull();
    }
}
