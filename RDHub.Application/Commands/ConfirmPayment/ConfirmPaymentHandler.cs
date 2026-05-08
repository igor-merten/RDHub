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
    private readonly IAccountRepository _accountRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IMessageQueue _messageQueue;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPaymentHandler(
        IAccountRepository accountRepository,
        IBankAdapterFactory adapterFactory,
        IMessageQueue messageQueue,
        IAuditRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _adapterFactory = adapterFactory;
        _messageQueue = messageQueue;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmPaymentResult> Handle(ConfirmPaymentCommand cmd, CancellationToken ct)
    {
        // busca na auditoria accoundId e bankId pelo txid
        var txId = TxId.From(cmd.TxId);
        var audits = await _auditRepository.GetByTxIdAsync(txId, ct);

        if (!audits.Any())
            throw new KeyNotFoundException("Cobrança não encontrada");

        // verifica se já está pago
        var alreadyPaid = audits.Any(a => a.Status == "Paid");
        if (alreadyPaid)
        {
            var paidAudit = audits.First(a => a.Status == "Paid");
            return new ConfirmPaymentResult(
                TxId: cmd.TxId,
                Status: "Paid",
                PaidAmount: paidAudit.PaidAmount,
                PaidAt: paidAudit.PaidAt);
        }

        // busca a audit de Invoice criada para obter valor e AccountId
        var invoiceAudit = audits.FirstOrDefault(a => a.Status == "Open")
            ?? throw new Exception("Audit de invoice não encontrada");

        // busca conta associada
        var account = await _accountRepository.GetByIdAsync(invoiceAudit.AccountId, ct)
            ?? throw new Exception("Conta não encontrada");

        // consulta o status no banco (mockserver)
        var adapter = _adapterFactory.Get(account.BankId.ToString());
        var bankStatus = await adapter.GetChargeStatusAsync(cmd.TxId, ct);

        // se não está pago, retorna o status atual
        if (bankStatus.Status != "Paid")
            return new ConfirmPaymentResult(
                TxId: cmd.TxId,
                Status: bankStatus.Status,
                PaidAmount: null,
                PaidAt: null);


        // valida se valor pago corresponde ao valor da fatura
        //if (invoiceAudit.Amount.HasValue && bankStatus.PaidAmount.HasValue)
        //{
        //    if (Math.Abs(bankStatus.PaidAmount.Value - invoiceAudit.Amount.Value) > 0.01m)
        //        throw new Exception($"Valor pago {bankStatus.PaidAmount} diverge do valor da fatura {invoiceAudit.Amount}");
        //}

        // registra auditoria
        await _auditRepository.AddAsync(Audit.Create(
            accountId: invoiceAudit.AccountId,
            action: "Pagamento confirmado",
            detail: $"TxId={cmd.TxId}, Valor={bankStatus.PaidAmount}",
            txId: cmd.TxId,
            amount: invoiceAudit.Amount,
            currency: "BRL",
            status: "Paid",
            paidAmount: bankStatus.PaidAmount,
            paidAt: bankStatus.PaidAt), ct);

        // persiste no banco de dados
        await _unitOfWork.SaveChangesAsync(ct);

        // publica na fila
        await _messageQueue.PublishAsync("payment-confirmed", new
        {
            TxId = cmd.TxId,
            AccountId = invoiceAudit.AccountId,
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
