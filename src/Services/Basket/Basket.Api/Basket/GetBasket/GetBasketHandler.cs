namespace Basket.Api.Basket.GetBasket;

public record GetBasketQuery(string UserName)
    : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCart Cart)
{
    public static GetBasketResponse ToResponse(
        GetBasketResult result
    ) => new(Cart: result.Cart);
}

public class GetBasketHandler(IBasketRepository repository)
    : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(
        GetBasketQuery query,
        CancellationToken cancellationToken
    )
    {
        var cart = await repository.GetBasketAsync(
            userName: query.UserName,
            cancellationToken: cancellationToken
        );

        return new GetBasketResult(Cart: cart);
    }
}
