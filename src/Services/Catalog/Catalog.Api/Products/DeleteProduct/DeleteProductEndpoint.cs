namespace Catalog.Api.Products.DeleteProduct;

public record DeleteProductRequest(Guid Id)
{
    public static DeleteProductCommand ToCommand(
        DeleteProductRequest req
    ) => new(req.Id);
}

public record DeleteProductResponse(bool Success)
{
    public static DeleteProductResponse ToResponse(
        DeleteProductResult result
    ) => new(result.Success);
}

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete(
                "/products/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var command = DeleteProductRequest.ToCommand(
                        new DeleteProductRequest(id)
                    );

                    var result = await sender.Send(command);

                    var response = DeleteProductResponse.ToResponse(
                        result
                    );

                    return Results.Ok(response);
                }
            )
            .WithName("Delete Product")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Product")
            .WithDescription("Delete a product.");
}
