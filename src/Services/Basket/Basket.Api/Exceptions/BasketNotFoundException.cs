namespace Basket.Api.Data;

public class BasketNotFoundException : NotFoundException
{
    public BasketNotFoundException(string userName)
        : base(name: "Basket", key: userName) { }
}
