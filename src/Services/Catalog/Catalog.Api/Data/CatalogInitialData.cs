namespace Catalog.Api.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate(
        IDocumentStore store,
        CancellationToken cancellation
    )
    {
        await using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync(cancellation))
        {
            return;
        }

        session.Store(GetPreconfiguredProduct());
        await session.SaveChangesAsync(cancellation);
    }

    private static IEnumerable<Product> GetPreconfiguredProduct() =>
        new List<Product>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Keyboard",
                Category = new List<string>
                {
                    "Electronics",
                    "Hardware",
                },
                Description = "A mechanical keyboard",
                Price = 50.00m,
                ImageFile = "keyboard.jpg",
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                Category = new List<string>
                {
                    "Electronics",
                    "Hardware",
                },
                Description = "A wireless mouse",
                Price = 20.00m,
                ImageFile = "mouse.jpg",
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Monitor",
                Category = new List<string>
                {
                    "Electronics",
                    "Hardware",
                },
                Description = "A 4k monitor",
                Price = 300.00m,
                ImageFile = "monitor.jpg",
            },
        };
}
