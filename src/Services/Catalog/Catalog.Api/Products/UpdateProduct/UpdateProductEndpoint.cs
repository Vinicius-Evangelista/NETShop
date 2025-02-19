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
            request.Id,
            request.Name,
            request.Category,
            request.Description,
            request.Price,
            request.ImageFile
        );
}

public record UpdateProductResponse(bool Success)
{
    public static UpdateProductResponse ToResponse(
        UpdateProductResult result
    ) => new(result.Success);
}

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut(
                "/products",
                async (UpdateProductRequest req, ISender sender) =>
                {
                    var result = await sender.Send(
                        UpdateProductRequest.ToCommand(req)
                    );

                    var response = UpdateProductResponse.ToResponse(
                        result
                    );

                    return Results.Created(
                        $"/products/{req.Id}",
                        response
                    );
                }
            )
            .WithName("Update Product")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update a Product")
            .WithDescription("Update a product by id");
}
