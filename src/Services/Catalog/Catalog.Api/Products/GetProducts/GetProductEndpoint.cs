namespace Catalog.Api.Products.GetProducts;

public record GetProductRequest();

public record GetProductResponse(IEnumerable<Product> Products)
{
    public static GetProductResponse ToResponse(GetProductResult result) =>
        new GetProductResponse(result.Products);
}

public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                "/products",
                async (ISender sender) =>
                {
                    var result = await sender.Send(new GetProductQuery());

                    var response = GetProductResponse.ToResponse(result);

                    return Results.Ok(response);
                }
            )
            .WithName("Get products")
            .Produces<GetProductResponse>(StatusCodes.Status200OK)
            .WithSummary("Get a list of products")
            .WithDescription("Get a list of products");
}
