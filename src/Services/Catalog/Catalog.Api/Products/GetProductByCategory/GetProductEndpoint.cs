namespace Catalog.Api.Products.GetProductByCategory;

public record GetProductRequest();

public record GetProductResponse(IEnumerable<Product> Products)
{
    public static GetProductResponse ToResponse(
        GetProductResult result
    ) => new(Products: result.Products);
}

public class GetProductByCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                pattern: "/products/{category}",
                handler: async (string category, ISender sender) =>
                {
                    var result = await sender.Send(
                        request: new GetProductByCategoryQuery(
                            Category: category
                        )
                    );

                    var response = GetProductResponse.ToResponse(
                        result: result
                    );

                    return Results.Ok(value: response);
                }
            )
            .WithName(endpointName: "Get Product by Category")
            .Produces<GetProductResponse>(
                statusCode: StatusCodes.Status200OK
            )
            .WithSummary(
                summary: "Get a list of products by its category"
            )
            .WithDescription(
                description: "Get a list of by its category"
            );
}
