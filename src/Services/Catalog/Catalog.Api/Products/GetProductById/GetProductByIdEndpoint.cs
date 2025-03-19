namespace Catalog.Api.Products.GetProductById;

public record GetProductRequest();

public record GetProductResponse(Product Product)
{
    public static GetProductResponse ToResponse(
        GetProductResult result
    ) => new GetProductResponse(result.Product);
}

public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                "/products/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetProductQuery(id)
                    );

                    var response = GetProductResponse.ToResponse(
                        result
                    );

                    return Results.Ok(response);
                }
            )
            .WithName("GetProductById")
            .Produces<GetProductResponse>(StatusCodes.Status200OK)
            .WithSummary("Get a Product by its id")
            .WithDescription("Get a Product by its id");
}
