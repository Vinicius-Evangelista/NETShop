namespace Catalog.Api.Products.UpdateProduct;

public record UpdateProductRequest(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    decimal Price,
    string ImageFile
)
{
    public static UpdateProductCommand ToCommand(
        UpdateProductRequest request
    ) =>
        new(
            Id: request.Id,
            Name: request.Name,
            Category: request.Category,
            Description: request.Description,
            Price: request.Price,
            ImageFile: request.ImageFile
        );
}

public record UpdateProductResponse(bool Success)
{
    public static UpdateProductResponse ToResponse(
        UpdateProductResult result
    ) => new(Success: result.Success);
}

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut(
                pattern: "/products",
                handler: async (
                    UpdateProductRequest req,
                    ISender sender
                ) =>
                {
                    var result = await sender.Send(
                        request: UpdateProductRequest.ToCommand(
                            request: req
                        )
                    );

                    var response = UpdateProductResponse.ToResponse(
                        result: result
                    );

                    return Results.Created(
                        uri: $"/products/{req.Id}",
                        value: response
                    );
                }
            )
            .WithName(endpointName: "Update Product")
            .Produces<UpdateProductResponse>(
                statusCode: StatusCodes.Status200OK
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status404NotFound
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status400BadRequest
            )
            .WithSummary(summary: "Update a Product")
            .WithDescription(description: "Update a product by id");
}
