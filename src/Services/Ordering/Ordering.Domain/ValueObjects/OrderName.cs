namespace Ordering.Domain.ValueObjects;

public record OrderName
{
    public string Value { get; } = default!;

    private OrderName(string value) => Value = value;

    public static OrderName Of(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new DomainException(
                "OrderName must be a non-empty string")
            : new OrderName(value);
}
