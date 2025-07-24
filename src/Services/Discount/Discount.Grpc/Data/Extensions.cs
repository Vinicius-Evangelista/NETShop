using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public static class Extensions
{
    public static IApplicationBuilder UseMigration(
        this IApplicationBuilder servicesApp
    )
    {
        using var scope =
            servicesApp.ApplicationServices.CreateScope();

        using var dbContext =
            scope.ServiceProvider.GetRequiredService<DiscountDbContext>();

        dbContext.Database.MigrateAsync();

        return servicesApp;
    }
}
