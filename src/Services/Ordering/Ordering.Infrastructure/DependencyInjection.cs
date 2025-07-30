namespace Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString(
            name: "Database"
        );

        services.AddDbContext<OrderingDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
