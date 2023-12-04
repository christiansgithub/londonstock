using LondonStock.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LondonStock.Api.Tests;

public class WebApplicationFactoryWithInMemoryDb : WebApplicationFactory<Program>
{
    private readonly string _connectionString = "DataSource=:memory:";
    private readonly SqliteConnection _connection;

    public WebApplicationFactoryWithInMemoryDb()
    {
        _connection = new SqliteConnection(_connectionString);
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(services =>
        {
            services
                .AddEntityFrameworkSqlite()
                .AddDbContext<LondonStockContext>(options =>
                {
                    options.UseSqlite(_connection);
                    options.UseInternalServiceProvider(services.BuildServiceProvider());
                });

            using var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<LondonStockContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Close();
    }
}
