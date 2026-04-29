using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Commands.ConfirmPayment;

// organiza confirmação de pagamento: consulta o banco, atualiza dominio e notifica via fila
public sealed class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResult>
{
    private readonly IPixChargeRepository _pixChargeRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IMessageQueue _messageQueue;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPaymentHandler(
        IPixChargeRepository pixChargeRepository,
        IInvoiceRepository invoiceRepository,
        IBankAdapterFactory adapterFactory,
        IMessageQueue messageQueue,
        IAuditRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _pixChargeRepository = pixChargeRepository;
        _invoiceRepository = invoiceRepository;
        _adapterFactory = adapterFactory;
        _messageQueue = messageQueue;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmPaymentResult> Handle(ConfirmPaymentCommand cmd, CancellationToken ct)
    {
        // busca pix charge pelo txid
        var txId = TxId.From(cmd.TxId);
        var pixCharge = await _pixChargeRepository.GetByTxIdAsync(txId, ct)
            ?? throw new Exception("Cobrança não encontrada");

        // consulta o status no banco (mockserver)
        var adapter = _adapterFactory.Get(pixCharge.BankId);
        var bankStatus = await adapter.GetChargeStatusAsync(cmd.TxId, ct);

        // se não está pago, retorna o status atual
        if (bankStatus.Status != "paid")
            return new ConfirmPaymentResult(
                TxId: cmd.TxId,
                Status: bankStatus.Status,
                PaidAmount: null,
                PaidAt: null);

        // busca invoice associada
        var invoice = await _invoiceRepository.GetByIdAsync(pixCharge.InvoiceId, ct)
            ?? throw new Exception("Fatura não encontrada");

        // confirma pagamento no dominio
        pixCharge.ConfirmPayment(bankStatus.PaidAmount ?? 0, bankStatus.PaidAt ?? DateTime.UtcNow);
        invoice.MarkAsPaid(bankStatus.PaidAt ?? DateTime.UtcNow, Domain.ValueObjects.Money.BRL(bankStatus.PaidAmount ?? 0));

        // registra auditoria
        await _auditRepository.AddAsync(Audit.Create(
            action: "Pagamento confirmado",
            details: $"TxId={cmd.TxId}, Valor={bankStatus.PaidAmount}",
            userId: invoice.UserId), ct);

        // persiste invoice e pix charge juntos
        await _pixChargeRepository.UpdateAsync(pixCharge, ct);
        await _invoiceRepository.UpdateAsync(invoice, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // publica na fila
        await _messageQueue.PublishAsync("payment-confirmed", new
        {
            InvoiceId = invoice.Id,
            TxId = cmd.TxId,
            PaidAmount = bankStatus.PaidAmount,
            PaidAt = bankStatus.PaidAt
        }, ct);

        return new ConfirmPaymentResult(
            TxId: cmd.TxId,
            Status: "Paid",
            PaidAmount: bankStatus.PaidAmount,
            PaidAt: bankStatus.PaidAt);
    }
}
