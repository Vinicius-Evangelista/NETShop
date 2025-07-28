namespace Ordering.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services
    )
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();

        return services;
    }

    public static WebApplication UseApplicationServices(
        this WebApplication app
    )
    {
        app.MapControllers();
        app.MapHealthChecks(pattern: "/health");

        return app;
    }
}
