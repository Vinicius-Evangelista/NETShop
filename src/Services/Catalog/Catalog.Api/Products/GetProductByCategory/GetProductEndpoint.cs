namespace Catalog.Api.Products.GetProductByCategory;

public record GetProductRequest();

public record GetProductResponse(IEnumerable<Product> Products)
{
    public static GetProductResponse ToResponse(GetProductResult result) =>
        new GetProductResponse(result.Products);
}

public class GetProductByCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                "/products/{category}",
                async (string category, ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetProductByCategoryQuery(category)
                    );

                    var response = GetProductResponse.ToResponse(result);

                    return Results.Ok(response);
                }
            )
            .WithName("Get Product by Category")
            .Produces<GetProductResponse>(StatusCodes.Status200OK)
            .WithSummary("Get a list of products by its category")
            .WithDescription("Get a list of by its category");
}
