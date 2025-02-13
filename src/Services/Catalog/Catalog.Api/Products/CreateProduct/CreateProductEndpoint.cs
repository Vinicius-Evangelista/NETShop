namespace Catalog.Api.Products.CreateProduct;

public record CreateProductRequest(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
);

public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost(
                "/products",
                async (CreateProductRequest req, ISender sender) =>
                {
                    var command = CreateProductCommand.FromRequest(req);

                    var result = await sender.Send(command);

                    var response = CreateProductResult.ToResponse(result);

                    return Results.Created(
                        $"/products/{response.Id}",
                        response
                    );
                }
            )
            .WithName("Create Product")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithSummary("Create Product")
            .WithDescription("Create a product.");
}
