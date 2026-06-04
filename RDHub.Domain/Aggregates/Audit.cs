using RDHub.Domain.Exceptions;
using RDHub.Domain.ValueObjects;

namespace RDHub.Domain.Aggregates;

// Registro imutável de operações realizadas no sistema
public class Audit : AggregateRoot<Guid>
{
    public Guid AccountId { get; private set; }
    public string? TxId { get; private set; }
    public decimal Amount { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime? PaymentConfirmationTime { get; private set; }
    public Guid? PaymentId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Audit() { }

    public static Audit Create(
        Guid accountId,
        string status,
        decimal amount,
        string? txId = null,
        DateTime? paymentConfirmationTime = null)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("AccountId é obrigatório");
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status é obrigatório");
        if (amount <= 0)
            throw new DomainException("Amount é obrigatório");


        return new Audit
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Status = status,
            TxId = txId,
            Amount = amount,
            PaymentConfirmationTime = paymentConfirmationTime,
            CreatedAt = DateTime.UtcNow,
            PaymentId = null
        };
    }

    public void MarkAsPaid()
    {
        Status = "Paid";
        PaymentConfirmationTime = DateTime.UtcNow;
        PaymentId = Guid.NewGuid();
    }
}
