namespace Basket.Api.Data;

public class CachedBasketRepository(
    IBasketRepository repository,
    IDistributedCache cache
) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasketAsync(
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        var cachedBasket = await cache.GetStringAsync(
            key: userName,
            token: cancellationToken
        );

        if (!string.IsNullOrEmpty(value: cachedBasket))
        {
            return JsonSerializer.Deserialize<ShoppingCart>(
                json: cachedBasket
            );
        }

        var basket = await repository.GetBasketAsync(
            userName: userName,
            cancellationToken: cancellationToken
        );

        await cache.SetStringAsync(
            key: userName,
            value: JsonSerializer.Serialize(value: basket),
            token: cancellationToken
        );

        return basket;
    }

    public async Task<ShoppingCart> StoreBasketAsync(
        ShoppingCart basket,
        CancellationToken cancellationToken = default
    )
    {
        await repository.StoreBasketAsync(
            basket: basket,
            cancellationToken: cancellationToken
        );

        await cache.SetStringAsync(
            key: basket.UserName,
            value: JsonSerializer.Serialize(value: basket),
            token: cancellationToken
        );

        return basket;
    }

    public async Task<bool> DeleteBasketAsync(
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        await repository.DeleteBasketAsync(
            userName: userName,
            cancellationToken: cancellationToken
        );

        await cache.RemoveAsync(
            key: userName,
            token: cancellationToken
        );

        return true;
    }
}
