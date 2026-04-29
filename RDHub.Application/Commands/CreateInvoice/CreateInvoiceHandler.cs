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
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPixChargeRepository _pixChargeRepository;

    public CreateInvoiceHandler(
        IInvoiceRepository invoiceRepository,
        IPixChargeRepository pixChargeRepository,
        IUserRepository userRepository,
        IBankAdapterFactory adapterFactory,
        IAuditRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _pixChargeRepository = pixChargeRepository;
        _userRepository = userRepository;
        _adapterFactory = adapterFactory;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateInvoiceResult> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
    {
        // busca usuario para obter BankId e PixKey
        var user = await _userRepository.GetByIdAsync(cmd.UserId, ct)
            ?? throw new Exception("Usuário não encontrado");

        // cria invoice no domínio
        var invoice = Invoice.Create(cmd.UserId, Money.BRL(cmd.Amount), user.BankId);

        // gera TxId único
        var txId = TxId.Generate();

        // obtem adapter do banco correto
        var adapter = _adapterFactory.Get(user.BankId);

        // cria cobrança 
        var bankResponse = await adapter.CreateChargeAsync(new BankChargeRequest(
            TxId: txId.Value,
            Amount: cmd.Amount,
            PixKey: user.PixKey), ct);

        // cria pix charge com o qr code retornado pelo banco
        var pixCharge = PixCharge.Create(txId, invoice.Id, user.BankId, bankResponse.Emv);

        // associa txid à invoice
        invoice.AssignTxId(txId);

        // registra auditoria
        await _auditRepository.AddAsync(Audit.Create(
            action: "Invoice criada",
            details: $"UserId={cmd.UserId}, Valor={cmd.Amount}, BankId={user.BankId}",
            userId: cmd.UserId), ct);

        //persiste invoice e pix charge juntos
        await _invoiceRepository.AddAsync(invoice, ct);
        await _pixChargeRepository.AddAsync(pixCharge, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateInvoiceResult(
            InvoiceId: invoice.Id,
            TxId: txId.Value,
            Status: invoice.Status.ToString(),
            Emv: bankResponse.Emv);
    }
}
