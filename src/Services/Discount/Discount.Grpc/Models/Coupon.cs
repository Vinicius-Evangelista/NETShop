namespace Discount.Grpc.Models;

public class Coupon
{
    public Coupon() { }

    public Coupon(
        int id,
        string productName,
        string description,
        int quantity
    )
    {
        Id = id;
        ProductName = productName;
        Description = description;
        Quantity = quantity;
    }

    public int Id { get; set; }
    public string ProductName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Quantity { get; set; }
}
