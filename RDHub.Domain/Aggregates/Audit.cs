using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates;

// Registro imutável de operações realizadas no sistema
public class Audit : AggregateRoot<Guid>
{
    public string Action { get; private set; } = null!;
    public Guid? UserId { get; private set; }
    public string Details { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Audit() { }

    public static Audit Create(string action, string details, Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Ação é obrigatória");

        if (string.IsNullOrWhiteSpace(details))
            throw new DomainException("Detalhes são obrigatórios");

        return new Audit
        {
            Id = Guid.NewGuid(),
            Action = action,
            Details = details,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }
}