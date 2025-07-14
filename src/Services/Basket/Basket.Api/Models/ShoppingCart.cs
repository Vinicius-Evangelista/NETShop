namespace Basket.Api.Models;

public class ShoppingCart
{
    public string UserName { get; set; } = default!;

    public List<ShoppingCartItem> Items { get; set; } = [];

    public decimal TotalPrice =>
        Items.Sum(selector: x => x.Price * x.Quantity);

    public ShoppingCart(string userName) => UserName = userName;

    public ShoppingCart() { }
}
