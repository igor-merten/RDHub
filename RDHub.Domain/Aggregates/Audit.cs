using RDHub.Domain.Exceptions;
using RDHub.Domain.ValueObjects;

namespace RDHub.Domain.Aggregates;

// Registro imutável de operações realizadas no sistema
public class Audit : AggregateRoot<Guid>
{
    public Guid AccountId { get; private set; }
    public Guid BankId { get; private set; }
    public string Action { get; private set; } = null!;
    public string Detail { get; private set; } = null!;
    public string? TxId { get; private set; }
    public decimal? Amount { get; private set; }
    public string? Currency { get; private set; }
    public string? Status { get; private set; }
    public decimal? PaidAmount { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Audit() { }

    public static Audit Create(
        Guid accountId,
        Guid bankId,
        string action,
        string detail,
        string? txId = null,
        decimal? amount = null,
        string? currency = null,
        string? status = null,
        decimal? paidAmount = null,
        DateTime? paidAt = null)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("AccountId é obrigatório");

        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Ação é obrigatória");

        if (string.IsNullOrWhiteSpace(detail))
            throw new DomainException("Detalhe é obrigatório");

        return new Audit
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            BankId = bankId,
            Action = action,
            Detail = detail,
            TxId = txId,
            Amount = amount,
            Currency = currency,
            Status = status,
            PaidAmount = paidAmount,
            PaidAt = paidAt,
            CreatedAt = DateTime.UtcNow
        };
    }
}