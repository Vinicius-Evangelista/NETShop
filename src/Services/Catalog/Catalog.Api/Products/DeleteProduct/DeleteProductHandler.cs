namespace Catalog.Api.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id)
    : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool Success);

internal class DeleteProductHandler(
    IDocumentSession dbSession,
    ILogger<DeleteProductHandler> logger
) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken
    )
    {
        dbSession.Delete<Product>(request.Id);

        await dbSession.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}
