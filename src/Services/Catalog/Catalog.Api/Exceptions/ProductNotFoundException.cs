using BuildingBlocks.Exceptions;

namespace Catalog.Api.Exceptions;

public class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(Guid id)
        : base(name: "Product", key: id) { }
}
