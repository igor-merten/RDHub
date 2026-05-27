using MediatR;
using Microsoft.VisualBasic;
using RDHub.Application.DTOs;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Exceptions;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace RDHub.Application.Commands.CreateCobv;

public sealed class CreateCobvHandler : IRequestHandler<CreateCobvCommand, CreateCobvResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IAuditRepository _auditRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCobvHandler(
        IAccountRepository accountRepository,
        IBankAdapterFactory adapterFactory,
        IAuditRepository auditRepository,
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _adapterFactory = adapterFactory;
        _auditRepository = auditRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCobvResult> Handle(CreateCobvCommand cmd, CancellationToken ct)
    {
        var account = await _accountRepository.GetByPixKeyAsync(cmd.PixKey, ct)
            ?? throw new DomainException("Conta não encontrada");

        var invoice = Invoice.Create(cmd.InvoiceId, Money.BRL(cmd.Amount), cmd.DueDate);

        var txId = TxId.Generate();

        var adapter = _adapterFactory.Get(account.BankId.ToString());

        // monta request
        var bankRequest = new BankChargeRequest(
            TxId: txId.Value,
            Type: PixChargeType.CobV,
            Amount: cmd.Amount,
            PixKey: cmd.PixKey,
            DueDate: cmd.DueDate,
            PayerMessage: cmd.PayerMessage);

        // manda request e recebe response do banco
        var bankResponse = await adapter.CreateCob(bankRequest, account.Credential, ct);

        var pixCharge = PixCharge.Create(txId, invoice.Id, account.BankId.ToString(), bankResponse.Emv);

        invoice.AssignTxId(txId);

        // cria auditoria
        var audit = Audit.Create(
            accountId: account.Id,
            status: invoice.Status.ToString(),
            txId: txId.Value,
            amount: cmd.Amount);

        // salva auditoria
        await _auditRepository.AddAsync(audit, ct);

        await _messageRepository.AddAsync(Message.Create(
            auditoryId: audit.Id,
            description: "Solicitação de criação de cobrança",
            type: "Request",
            body: JsonSerializer.SerializeToElement(bankRequest)), ct);


        await _messageRepository.AddAsync(Message.Create(
            auditoryId: audit.Id,
            description: "Solicitação de criação de cobrança",
            type: "Response",
            body: JsonSerializer.SerializeToElement(bankResponse)), ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateCobvResult(
            TxId: txId.Value,
            Status: invoice.Status.ToString(),
            Emv: bankResponse.Emv,
            PixLink: $"pix.example.com/{txId.Value}");
    }
}