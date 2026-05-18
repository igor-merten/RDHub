using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Application.DTOs;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;

namespace RDHub.Application.Commands.CreateCobv;

//cria cobv
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
        // busca usuario para obter BankId e PixKey
        var account = await _accountRepository.GetByIdAsync(cmd.InvoiceId, ct)
            ?? throw new Exception("Conta não encontrada");

        // cria invoice no domínio
        var invoice = Invoice.Create(cmd.InvoiceId, Money.BRL(cmd.Amount));

        // gera TxId único
        var txId = TxId.Generate();

        // obtem adapter do banco
        var adapter = _adapterFactory.Get(account.BankId.ToString());

        // cria cobrança e recebe response
        var bankResponse = await adapter.CreateChargeAsync(new BankChargeRequest(
            TxId: txId.Value,
            Amount: cmd.Amount), ct);

        // cria pix charge com o qr code retornado pelo banco
        var pixCharge = PixCharge.Create(txId, invoice.Id, account.BankId.ToString(), bankResponse.Emv);

        // associa txid à invoice
        invoice.AssignTxId(txId);

        // registra auditoria
        await _auditRepository.AddAsync(Audit.Create(
            accountId: cmd.InvoiceId,
            action: "Cobv criada",
            detail: $"InvoiceId={cmd.InvoiceId}, Valor={cmd.Amount}, PixKey={cmd.PixKey}, Tipo={cmd.ChargeType}, Vencimento={cmd.DueDate:yyyy-MM-dd}",
            txId: txId.Value,
            amount: cmd.Amount,
            currency: "BRL",
            status: invoice.Status.ToString()), ct);

        //persiste no banco de dados
        await _unitOfWork.SaveChangesAsync(ct);

        //devolve response para rd
        return new CreateCobvResult(
            TxId: txId.Value,
            Status: invoice.Status.ToString(),
            Emv: bankResponse.Emv,
            PixLink: $"pix.example.com/{txId.Value}");
    }
}
