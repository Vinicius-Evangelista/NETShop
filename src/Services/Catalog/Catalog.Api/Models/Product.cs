namespace Catalog.Api;

public class Product
{
    // TODO: verify the use o strongly typed id's concept
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public List<string> Category { get; set; } = [];

    public string Description { get; set; } = default!;

    public string ImageFile { get; set; } = default!;

    public decimal Price { get; set; }
}
