using LondonStock.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using LondonStock.Api.Stocks;
using LondonStock.Api.Trades;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlLiteInMemoryDataContext();

builder.Services.AddValidatorsFromAssemblyContaining<TradeDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<TradeReceiver>();
builder.Services.AddScoped<StockInformation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
