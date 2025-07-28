namespace Ordering.Domain.Abstractions;

public interface IDomainEvent
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName!;
}
