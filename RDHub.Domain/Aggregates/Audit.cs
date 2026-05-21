using RDHub.Domain.Exceptions;
using RDHub.Domain.ValueObjects;

namespace RDHub.Domain.Aggregates;

// Registro imutável de operações realizadas no sistema
public class Audit : AggregateRoot<Guid>
{
    public Guid AccountId { get; private set; }
    public string Payloads { get; private set; } = null!;
    public string? TxId { get; private set; }
    public decimal? Amount { get; private set; }
    public string? Status { get; private set; }
    public DateTime? PaymentConfirmationTime { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Audit() { }

    public static Audit Create(
        Guid accountId,
        string payloads,
        string? txId = null,
        decimal? amount = null,
        string? status = null,
        DateTime? paymentConfirmationTime = null)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("AccountId é obrigatório");
        if (string.IsNullOrWhiteSpace(payloads))
            throw new DomainException("Payload é obrigatório");

        return new Audit
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Payloads = payloads,
            TxId = txId,
            Amount = amount,
            Status = status,
            PaymentConfirmationTime = paymentConfirmationTime,
            CreatedAt = DateTime.UtcNow
        };
    }
}