namespace Catalog.Api.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id)
    : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool Success);

class DeleteProductHandler(IDocumentSession dbSession)
    : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken
    )
    {
        dbSession.Delete<Product>(id: request.Id);

        await dbSession.SaveChangesAsync(token: cancellationToken);

        return new DeleteProductResult(Success: true);
    }
}
