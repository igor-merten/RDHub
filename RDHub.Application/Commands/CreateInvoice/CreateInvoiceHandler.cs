using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;
using RDHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Commands.CreateInvoice;

// organiza criação de fatura e cobrança no banco
public sealed class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceHandler(
        IAccountRepository accountRepository,
        IBankAdapterFactory adapterFactory,
        IAuditRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _adapterFactory = adapterFactory;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateInvoiceResult> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
    {
        // busca usuario para obter BankId e PixKey
        var account = await _accountRepository.GetByIdAsync(cmd.AccountId, ct)
            ?? throw new Exception("Conta não encontrada");

        // cria invoice no domínio
        var invoice = Invoice.Create(cmd.AccountId, Money.BRL(cmd.Amount));

        // gera TxId único
        var txId = TxId.Generate();

        // obtem adapter do banco correto
        var adapter = _adapterFactory.Get(account.BankId.ToString());

        // cria cobrança 
        var bankResponse = await adapter.CreateChargeAsync(new BankChargeRequest(
            TxId: txId.Value,
            Amount: cmd.Amount,
            PixKey: account.PixKey), ct);

        // cria pix charge com o qr code retornado pelo banco
        var pixCharge = PixCharge.Create(txId, invoice.Id, account.BankId.ToString(), bankResponse.Emv);

        // associa txid à invoice
        invoice.AssignTxId(txId);

        // registra auditoria
        await _auditRepository.AddAsync(Audit.Create(
            accountId: cmd.AccountId,
            action: "Invoice criada",
            detail: $"AccountId={cmd.AccountId}, Valor={cmd.Amount}, BankId={account.BankId}",
            txId: txId.Value,
            amount: cmd.Amount,
            currency: "BRL",
            status: invoice.Status.ToString()), ct);

        //persiste no banco de dados
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateInvoiceResult(
            InvoiceId: invoice.Id,
            TxId: txId.Value,
            Status: invoice.Status.ToString(),
            Emv: bankResponse.Emv);
    }
}
