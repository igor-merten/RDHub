using RDHub.Domain.Exceptions;
using RDHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

// representa a intenção de cobrança criada pela Receba Digital
public class Invoice : AggregateRoot<Guid>
{
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public InvoiceStatus Status { get; private set; }
    public TxId? TxId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Invoice() { }

    public static Invoice Create(Guid accountId, Money amount)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("AccountId é obrigatório");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Amount = amount,
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
