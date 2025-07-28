namespace Ordering.Domain.ValueObjects;

public record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value) => Value = value;

    public static ProductId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return (value == Guid.Empty)
            ? throw new DomainException("ProductId must be a valid Guid")
            : new ProductId(value);
    }
}
