using RDHub.Domain.Exceptions;
using RDHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

public class PixCharge : AggregateRoot<Guid>
{
    public TxId TxId { get; private set; } = null!;
    public Guid InvoiceId { get; private set; }
    public string BankId { get; private set; } = null!;
    public PixChargeStatus Status { get; private set; }
    public string Emv { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public decimal? PaidAmount { get; private set; }

    private PixCharge() { }

    public static PixCharge Create(TxId txId, Guid invoiceId, string bankId, string emv)
    {
        if (string.IsNullOrEmpty(bankId))
            throw new DomainException("O banco deve ser informado.");
        if (string.IsNullOrEmpty(emv))
            throw new DomainException("O código EMV deve ser informado.");

        var pixCharge = new PixCharge
        {
            Id = Guid.NewGuid(),
            TxId = txId,
            InvoiceId = invoiceId,
            BankId = bankId,
            Emv = emv,
            Status = PixChargeStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        return pixCharge;
    }

    public void ConfirmPayment(decimal paidAmount, DateTime paidAt)
    {
        if (Status == PixChargeStatus.Paid)
            return;

        PaidAmount = paidAmount;
        PaidAt = paidAt;
        Status = PixChargeStatus.Paid;
    }

    public void Expire()
    {
        if (Status == PixChargeStatus.Paid)
            throw new DomainException("Não é possível expirar uma cobrança já paga.");

        Status = PixChargeStatus.Expired;
    }
}
