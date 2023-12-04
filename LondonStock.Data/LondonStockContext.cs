using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LondonStock.Data;

public class LondonStockContext : DbContext
{
    public DbSet<Trade> Trades { get; set; }

    public LondonStockContext(DbContextOptions<LondonStockContext> options)
        : base(options)
    {

    }
}

public static class LondonStockContextExtensions
{
    public static IServiceCollection AddSqlLiteInMemoryDataContext(this IServiceCollection services)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        services
            .AddEntityFrameworkSqlite()
            .AddDbContext<LondonStockContext>(
            options =>
            {
                options.UseSqlite(connection);
                options.UseInternalServiceProvider(services.BuildServiceProvider());
            });

        using var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<LondonStockContext>();
        dbContext.Database.EnsureCreated();

        return services;
    }
}
