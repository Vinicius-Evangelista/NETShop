namespace Catalog.Api.Products.GetProductById;

public record GetProductQuery(Guid Id) : IQuery<GetProductResult>;

public record GetProductResult(Product Product);

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
        var product = await session.LoadAsync<Product>(
            query.Id,
            cancellationToken
        );

        if (product is null)
        {
            throw new ProductNotFoundException();
        }

        return new GetProductResult(product);
    }
}
