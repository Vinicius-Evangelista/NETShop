using Marten.Patching;

namespace Catalog.Api.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    decimal Price,
    string ImageFile
) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool Success);

public class UpdateProductHandler(IDocumentSession session)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = await session.LoadAsync<Product>(
            request.Id,
            cancellationToken
        );

        if (product is null)
        {
            throw new ProductNotFoundException(product!.Id);
        }

        session
            .Patch<Product>(request.Id)
            .Set(x => x.Name, request.Name)
            .Set(x => x.Category, request.Category)
            .Set(x => x.Description, request.Description)
            .Set(x => x.Price, request.Price)
            .Set(x => x.ImageFile, request.ImageFile);

        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
