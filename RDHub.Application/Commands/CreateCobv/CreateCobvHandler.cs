using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Application.DTOs;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;

namespace RDHub.Application.Commands.CreateCobv;

public sealed class CreateCobvHandler : IRequestHandler<CreateCobvCommand, CreateCobvResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankAdapterFactory _adapterFactory;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCobvHandler(
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

    public async Task<CreateCobvResult> Handle(CreateCobvCommand cmd, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(cmd.InvoiceId, ct)
            ?? throw new Exception("Conta não encontrada");

        var invoice = Invoice.Create(Money.BRL(cmd.Amount), cmd.DueDate, account.BankId.ToString());

        var txId = TxId.Generate();

        var adapter = _adapterFactory.Get(account.BankId.ToString());

        var bankResponse = await adapter.CreateCobV(new BankChargeRequest(
            TxId: txId.Value,
            Amount: cmd.Amount), ct);

        var pixCharge = PixCharge.Create(txId, invoice.Id, account.BankId.ToString(), bankResponse.Emv);

        invoice.AssignTxId(txId);

        await _auditRepository.AddAsync(Audit.Create(
            accountId: cmd.InvoiceId,
            action: "Cobv criada",
            detail: $"InvoiceId={cmd.InvoiceId}, Valor={cmd.Amount}, PixKey={cmd.PixKey}, Tipo={cmd.ChargeType}, Vencimento={cmd.DueDate:yyyy-MM-dd}",
            txId: txId.Value,
            amount: cmd.Amount,
            currency: "BRL",
            status: invoice.Status.ToString()), ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateCobvResult(
            TxId: txId.Value,
            Status: invoice.Status.ToString(),
            Emv: bankResponse.Emv,
            PixLink: $"pix.example.com/{txId.Value}");
    }
}