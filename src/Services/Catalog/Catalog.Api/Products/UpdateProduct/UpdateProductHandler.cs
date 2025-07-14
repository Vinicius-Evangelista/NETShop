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
            id: request.Id,
            token: cancellationToken
        );

        if (product is null)
        {
            throw new ProductNotFoundException(id: product!.Id);
        }

        session
            .Patch<Product>(id: request.Id)
            .Set(expression: x => x.Name, value: request.Name)
            .Set(expression: x => x.Category, value: request.Category)
            .Set(
                expression: x => x.Description,
                value: request.Description
            )
            .Set(expression: x => x.Price, value: request.Price)
            .Set(
                expression: x => x.ImageFile,
                value: request.ImageFile
            );

        await session.SaveChangesAsync(token: cancellationToken);

        return new UpdateProductResult(Success: true);
    }
}
