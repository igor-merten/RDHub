using RDHub.Domain.Exceptions;
using RDHub.Domain.ValueObjects;

namespace RDHub.Domain.Aggregates;

// representa a intenção de cobrança criada pela Receba Digital
public class Invoice : AggregateRoot<Guid>
{
    public Money Amount { get; private set; } = null!;
    public DateOnly DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public TxId? TxId { get; private set; }
    public string? ExternalReference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string BankId { get; private set; } = null!;

    private Invoice() { }

    public static Invoice Create(
        Money amount,
        DateOnly dueDate,
        string bankId,
        string? externalReference = null)
    {
        if (amount.Value <= 0)
            throw new DomainException("O valor da fatura deve ser positivo");

        if (dueDate < DateOnly.FromDateTime(DateTime.Today))
            throw new DomainException("A data de vencimento não pode ser no passado");

        if (string.IsNullOrWhiteSpace(bankId))
            throw new DomainException("O banco deve ser informado");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            DueDate = dueDate,
            BankId = bankId,
            ExternalReference = externalReference,
            Status = InvoiceStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        return invoice;
    }

    public void AssignTxId(TxId txId)
    {
        if (TxId is not null)
            throw new DomainException("Fatura já possui TxId associado");

        TxId = txId;
    }

    public void MarkAsPaid(DateTime paidAt, Money paidAmount)
    {
        if (Status == InvoiceStatus.Paid)
            return;

        if (Status == InvoiceStatus.Canceled)
            throw new DomainException("Não é possível pagar uma fatura cancelada.");

        if (Math.Abs(paidAmount.Value - Amount.Value) > 0.01m)
            throw new DomainException("O valor pago não corresponde ao valor da fatura.");

        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Não é possível cancelar uma fatura paga.");

        Status = InvoiceStatus.Canceled;
    }
}