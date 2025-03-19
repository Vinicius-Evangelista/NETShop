namespace Catalog.Api.Products.GetProducts;

public record GetProductRequest(
    int? PageNumber = 1,
    int? PageSize = 10
);

public record GetProductResponse(IPagedList<Product> Products)
{
    public static GetProductResponse ToResponse(
        GetProductResult result
    ) => new(result.Products);
}

public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                "/products",
                async (
                    [AsParameters] GetProductRequest req,
                    ISender sender
                ) =>
                {
                    var result = await sender.Send(
                        new GetProductQuery()
                        {
                            PageNumber = req.PageNumber,
                            PageSize = req.PageSize,
                        }
                    );

                    var response = GetProductResponse.ToResponse(
                        result
                    );

                    return Results.Ok(response);
                }
            )
            .WithName("Get products")
            .Produces<GetProductResponse>(StatusCodes.Status200OK)
            .WithSummary("Get a list of products")
            .WithDescription("Get a list of products");
}
