using BuildingBlocks.CQRS;

namespace Catalog.Api.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<CreateProductResult>
{
    public static CreateProductCommand FromRequest(
        CreateProductRequest req
    ) =>
        new(
            Name: req.Name,
            Category: req.Category,
            Description: req.Description,
            ImageFile: req.ImageFile,
            Price: req.Price
        );
}

public record CreateProductResult(Guid Id)
{
    public static CreateProductResponse ToResponse(
        CreateProductResult req
    ) => new(req.Id);
}

internal class CreateProductHandler
    : ICommandHandler<
        CreateProductCommand,
        CreateProductResult
    >
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = new Product()
        {
            Name = request.Name,
            Category = request.Category,
            ImageFile = request.ImageFile,
            Description = request.Description,
            Price = request.Price,
        };

        // save entity to database


        return new CreateProductResult(Guid.NewGuid());

        // return CreateProductResult result
    }
}
