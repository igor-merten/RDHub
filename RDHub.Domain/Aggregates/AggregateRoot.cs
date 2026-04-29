using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

// Classe base para todos os aggregates: fornece Id e mecanismo de eventos
public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; } = default!;

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
