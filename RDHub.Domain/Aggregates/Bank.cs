using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates;

// Representa um banco disponível no HUB
public class Bank : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Bank() { }

    public static Bank Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do banco é obrigatório");

        return new Bank
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
    }
}