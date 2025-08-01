namespace BuildingBlocks.Messaging.Events;

public abstract class IntegrationEvent
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public DateTime CreationDate { get; private init; } = DateTime.UtcNow;
}
