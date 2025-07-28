namespace Ordering.Domain.Abstractions;

public abstract class Entity<T> : IEntity<T>
{
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public T Id { get; set; }
}
