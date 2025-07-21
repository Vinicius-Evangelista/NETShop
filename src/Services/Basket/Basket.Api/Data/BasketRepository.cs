namespace Basket.Api.Data;

public class BasketRepository(IDocumentSession session)
    : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasketAsync(
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        var basket = await session.LoadAsync<ShoppingCart>(
            id: userName,
            token: cancellationToken
        );

        return basket
            ?? throw new BasketNotFoundException(userName: userName);
    }

    public async Task<ShoppingCart> StoreBasketAsync(
        ShoppingCart basket,
        CancellationToken cancellationToken = default
    )
    {
        session.Store(basket);
        await session.SaveChangesAsync(token: cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasketAsync(
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        session.Delete<ShoppingCart>(id: userName);
        await session.SaveChangesAsync(token: cancellationToken);
        return true;
    }
}
