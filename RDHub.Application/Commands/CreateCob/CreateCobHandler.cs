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
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCobHandler(
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
        var bankRequest = new CobChargeRequestDto(
            TxId: txId.Value,
            Amount: cmd.Amount,
            PixKey: cmd.PixKey,
            ExpiresInSeconds: cmd.ExpireInSeconds,
            PayerMessage: cmd.PayerMessage);

        // manda request e recebe response do banco
        var bankResponse = await adapter.CreateCob(bankRequest, account.Credential, ct);

        // associa txId com invoice
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