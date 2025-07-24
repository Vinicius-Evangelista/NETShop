using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public class DiscountDbContext(
    DbContextOptions<DiscountDbContext> options
) : DbContext(options: options)
{
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder: modelBuilder);

        modelBuilder
            .Entity<Coupon>()
            .HasData(
                new Coupon(
                    id: 1,
                    productName: "IPhone X",
                    description: "IPhone Discount",
                    quantity: 1
                ),
                new Coupon(
                    id: 2,
                    productName: "Samsung S10",
                    description: "Samsung Discount",
                    quantity: 2
                )
            );
    }
}
