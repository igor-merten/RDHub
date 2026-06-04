using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RDHub.Application.Commands.ConfirmPayment;

// organiza confirmação de pagamento: consulta o banco, atualiza dominio e notifica via fila
public sealed class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IMessageQueue _messageQueue;
    private readonly IAuditRepository _auditRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPaymentHandler(
        IAccountRepository accountRepository,
        IBankAdapterFactory adapterFactory,
        IMessageQueue messageQueue,
        IAuditRepository auditRepository,
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _adapterFactory = adapterFactory;
        _messageQueue = messageQueue;
        _auditRepository = auditRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmPaymentResult> Handle(ConfirmPaymentCommand cmd, CancellationToken ct)
    {
        var txId = TxId.From(cmd.TxId);

        // busca na auditoria pelo txid
        var audit = await _auditRepository.GetByTxIdAsync(txId, ct)
            ?? throw new Exception("Auditoria não encontrada");

        // busca conta associada
        var account = await _accountRepository.GetByIdAsync(audit.AccountId, ct)
            ?? throw new Exception("Conta não encontrada");

        //// consulta o status no banco (mockserver)
        //var adapter = _adapterFactory.Get(account.BankId.ToString());
        //var bankStatus = await adapter.GetChargeStatusAsync(cmd.TxId, account.Credential, ct);

        // se não está pago, retorna o status atual
        if (cmd.Status != "Paid")
            return new ConfirmPaymentResult(
                TxId: cmd.TxId,
                isPaid: false,
                Status: cmd.Status,
                PaymentConfirmationTime: null);


        // valida se valor pago corresponde ao valor da fatura
        if (Math.Abs(cmd.PaidAmount - audit.Amount) > 0.01m)
            throw new Exception($"Valor pago {cmd.PaidAmount} diverge do valor da fatura {audit.Amount}");

        // registra auditoria
        audit.MarkAsPaid();

        await _messageRepository.AddAsync(Message.Create(
            auditoryId: audit.Id,
            description: "Confirmação de pagamento",
            type: "Response",
            body: JsonSerializer.SerializeToElement(cmd)), ct);

        // persiste no banco de dados
        await _unitOfWork.SaveChangesAsync(ct);

        // publica na fila
        await _messageQueue.PublishAsync("payment-confirmed", new
        {
            TxId = cmd.TxId,
            AccountId = audit.AccountId,
            PaidAmount = cmd.PaidAmount,
            PaymentConfirmationTime = DateTime.UtcNow
        }, ct);

        return new ConfirmPaymentResult(
            TxId: cmd.TxId,
            isPaid: true,
            Status: "Paid",
            PaymentConfirmationTime: DateTime.UtcNow);
    }
}
