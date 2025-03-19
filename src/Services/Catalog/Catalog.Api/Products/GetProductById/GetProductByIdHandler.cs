namespace Catalog.Api.Products.GetProductById;

public record GetProductQuery(Guid Id) : IQuery<GetProductResult>;

public record GetProductResult(Product Product);

public class GetProductQueryHandler(IDocumentSession session)
    : IQueryHandler<GetProductQuery, GetProductResult>
{
    public async Task<GetProductResult> Handle(
        GetProductQuery query,
        CancellationToken cancellationToken
    )
    {
        var product = await session.LoadAsync<Product>(
            query.Id,
            cancellationToken
        );

        if (product is null)
        {
            throw new ProductNotFoundException(product!.Id);
        }

        return new GetProductResult(product);
    }
}
