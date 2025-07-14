namespace Catalog.Api.Products.GetProducts;

public record GetProductRequest(
    int? PageNumber = 1,
    int? PageSize = 10
);

public record GetProductResponse(IPagedList<Product> Products)
{
    public static GetProductResponse ToResponse(
        GetProductResult result
    ) => new(Products: result.Products);
}

public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                pattern: "/products",
                handler: async (
                    [AsParameters] GetProductRequest req,
                    ISender sender
                ) =>
                {
                    var result = await sender.Send(
                        request: new GetProductQuery()
                        {
                            PageNumber = req.PageNumber,
                            PageSize = req.PageSize,
                        }
                    );

                    var response = GetProductResponse.ToResponse(
                        result: result
                    );

                    return Results.Ok(value: response);
                }
            )
            .WithName(endpointName: "Get products")
            .Produces<GetProductResponse>(
                statusCode: StatusCodes.Status200OK
            )
            .WithSummary(summary: "Get a list of products")
            .WithDescription(description: "Get a list of products");
}
