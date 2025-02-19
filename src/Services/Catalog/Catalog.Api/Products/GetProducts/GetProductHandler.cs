namespace Catalog.Api.Products.GetProducts;

public record GetProductQuery : IQuery<GetProductResult>;

public record GetProductResult(IEnumerable<Product> Products);

public class GetProductQueryHandler(
    ILogger<GetProductQueryHandler> logger,
    IDocumentSession session
) : IQueryHandler<GetProductQuery, GetProductResult>
{
    public async Task<GetProductResult> Handle(
        GetProductQuery query,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "GetProductQueryHandler.Handle called with query: {query}",
            query
        );

        var products = await session
            .Query<Product>()
            .ToListAsync(cancellationToken);

        return new GetProductResult(products);
    }
}
