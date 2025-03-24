using System.Threading;
using System.Threading.Tasks;

namespace Basket.Api.Basket.GetBasket;

public record GetBasketQuery(string UserName)
    : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCart Cart)
{
    public static GetBasketResponse ToResponse(
        GetBasketResult result
    ) => new GetBasketResponse(result.Cart);
}

public class GetBasketHandler
    : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(
        GetBasketQuery query,
        CancellationToken cancellationToken
    ) => new(new ShoppingCart("Vinicius"));
}
