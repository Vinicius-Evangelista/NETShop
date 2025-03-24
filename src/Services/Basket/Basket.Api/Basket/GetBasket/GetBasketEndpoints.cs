using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Basket.Api.Basket.GetBasket;

public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
            "/basket/{userName}",
            async (string userName, ISender sender) =>
            {
                var result = await sender.Send(
                    new GetBasketQuery(userName)
                );

                var response = GetBasketResult.ToResponse(result);
                return response;
            }
        );
}
