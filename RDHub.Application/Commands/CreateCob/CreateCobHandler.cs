using MediatR;
using RDHub.Application.DTOs;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Exceptions;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;
using System.Text.Json;

namespace RDHub.Application.Commands.CreateCob;

public sealed class CreateCobHandler : IRequestHandler<CreateCobCommand, CreateCobResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCobHandler(
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

    public async Task<CreateCobResult> Handle(CreateCobCommand cmd, CancellationToken ct)
    {
        // busca account pela pix key
        var account = await _accountRepository.GetByPixKeyAsync(cmd.PixKey, ct)
            ?? throw new DomainException("Conta não encontrada");

        // cria invoice
        var invoice = Invoice.Create(cmd.InvoiceId, Money.BRL(cmd.Amount));

        // gera txId
        var txId = TxId.Generate();

        // chama adapter do banco para criar a cobrança
        var adapter = _adapterFactory.Get(account.BankId.ToString());

        // monta request
        var bankRequest = new BankChargeRequest(
            TxId: txId.Value,
            Type: PixChargeType.Cob,
            Amount: cmd.Amount,
            PixKey: cmd.PixKey,
            ExpiresInSeconds: cmd.ExpireInSeconds,
            PayerMessage: cmd.PayerMessage);

        // manda request e recebe response do banco
        var bankResponse = await adapter.CreateCob(bankRequest);

        // monta payloads para auditoria
        var payloads = JsonSerializer.Serialize(new
        {
            request = bankRequest,
            response = bankResponse
        });

        // cria cobrança pix com os dados do banco
        // ver utilidade disso, já que o status da cobrança é controlado pelo banco e não pela aplicação
        var pixCharge = PixCharge.Create(txId, invoice.Id, account.BankId.ToString(), bankResponse.Emv);

        // associa txId com invoice
        invoice.AssignTxId(txId);

        // salva auditoria
        await _auditRepository.AddAsync(Audit.Create(
            accountId: account.Id,
            payloads: payloads,
            txId: txId.Value,
            amount: cmd.Amount,
            status: invoice.Status.ToString()), ct);

        // envia para o banco de dados
        await _unitOfWork.SaveChangesAsync(ct);

        // devolve resultado
        return new CreateCobResult(
            TxId: txId.Value,
            Status: invoice.Status.ToString(),
            Emv: bankResponse.Emv,
            PixLink: $"pix.example.com/{txId.Value}");
    }
}