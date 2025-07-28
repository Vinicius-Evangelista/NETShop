namespace Discount.Grpc.Models;

public class Coupon
{
    public Coupon() { }

    public Coupon(
        int id,
        string productName,
        string description,
        int amount
    )
    {
        Id = id;
        ProductName = productName;
        Description = description;
        Amount = amount;
    }

    public int Id { get; set; }
    public string ProductName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Amount { get; set; }
}
