namespace Catalog.Api.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<CreateProductResult>
{
    public static CreateProductCommand FromRequest(CreateProductRequest req) =>
        new(req.Name, req.Category, req.Description, req.ImageFile, req.Price);
}

public record CreateProductResult(Guid Id)
{
    public static CreateProductResponse ToResponse(CreateProductResult req) =>
        new(req.Id);
}

internal class CreateProductHandler(IDocumentSession dbSession)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = new Product
        {
            Name = request.Name,
            Category = request.Category,
            ImageFile = request.ImageFile,
            Description = request.Description,
            Price = request.Price,
        };

        dbSession.Store(product);
        await dbSession.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
