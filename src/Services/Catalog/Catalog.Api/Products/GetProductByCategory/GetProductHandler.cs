namespace Catalog.Api.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Category)
    : IQuery<GetProductResult>;

public record GetProductResult(IEnumerable<Product> Products);

public class GetProductQueryHandler(IDocumentSession session)
    : IQueryHandler<GetProductByCategoryQuery, GetProductResult>
{
    public async Task<GetProductResult> Handle(
        GetProductByCategoryQuery byCategoryQuery,
        CancellationToken cancellationToken
    )
    {
        var products = await session
            .Query<Product>()
            .Where(x => x.Category.Contains(byCategoryQuery.Category))
            .ToListAsync(cancellationToken);

        return new GetProductResult(products);
    }
}
