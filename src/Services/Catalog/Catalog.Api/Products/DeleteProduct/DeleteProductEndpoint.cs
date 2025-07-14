namespace Catalog.Api.Products.DeleteProduct;

public record DeleteProductRequest(Guid Id)
{
    public static DeleteProductCommand ToCommand(
        DeleteProductRequest req
    ) => new(Id: req.Id);
}

public record DeleteProductResponse(bool Success)
{
    public static DeleteProductResponse ToResponse(
        DeleteProductResult result
    ) => new(Success: result.Success);
}

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete(
                pattern: "/products/{id:guid}",
                handler: async (Guid id, ISender sender) =>
                {
                    var command = DeleteProductRequest.ToCommand(
                        req: new DeleteProductRequest(Id: id)
                    );

                    var result = await sender.Send(request: command);

                    var response = DeleteProductResponse.ToResponse(
                        result: result
                    );

                    return Results.Ok(value: response);
                }
            )
            .WithName(endpointName: "Delete Product")
            .Produces<DeleteProductResponse>(
                statusCode: StatusCodes.Status200OK
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status400BadRequest
            )
            .ProducesValidationProblem(
                statusCode: StatusCodes.Status404NotFound
            )
            .WithSummary(summary: "Delete Product")
            .WithDescription(description: "Delete a product.");
}
