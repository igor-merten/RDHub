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

public sealed class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResult>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPixChargeRepository _pixChargeRepository;

    public CreateInvoiceHandler(
        IInvoiceRepository invoiceRepository,
        IBankAdapterFactory adapterFactory,
        IUnitOfWork unitOfWork,
        IPixChargeRepository pixChargeRepository)
    {
        _invoiceRepository = invoiceRepository;
        _adapterFactory = adapterFactory;
        _unitOfWork = unitOfWork;
        _pixChargeRepository = pixChargeRepository;
    }

    public async Task<CreateInvoiceResult> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
    {
        // cria invoice no domínio
        var invoice = Invoice.Create(Money.BRL(cmd.Amount), cmd.BankId);

        // gera TxId único
        var txId = TxId.Generate();

        // obtem adapter do banco correto
        var adapter = _adapterFactory.Get(cmd.BankId);

        // cria cobrança 
        var bankResponse = await adapter.CreateChargeAsync(new BankChargeRequest(
            TxId: txId.Value,
            Amount: cmd.Amount,
            PixKey: cmd.PixKey), ct);

        // cria pix charge com o qr code retornado pelo banco
        var pixCharge = PixCharge.Create(txId: txId, invoiceId: invoice.Id, cmd.BankId, emv: bankResponse.Emv);

        // associa txid à invoice
        invoice.AssignTxId(txId);

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
