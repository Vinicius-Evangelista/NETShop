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
                pattern: "/products",
                handler: async (
                    CreateProductRequest req,
                    ISender sender
                ) =>
                {
                    var command = CreateProductCommand.FromRequest(
                        req: req
                    );

                    var result = await sender.Send(request: command);

                    var response = CreateProductResult.ToResponse(
                        req: result
                    );

                    return Results.Created(
                        uri: $"/products/{response.Id}",
                        value: response
                    );
                }
            )
            .WithName(endpointName: "Create Product")
            .Produces<CreateProductResponse>(
                statusCode: StatusCodes.Status201Created
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status500InternalServerError
            )
            .ProducesValidationProblem(
                statusCode: StatusCodes.Status422UnprocessableEntity
            )
            .WithSummary(summary: "Create Product")
            .WithDescription(description: "Create a product.");
}
