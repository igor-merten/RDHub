using MediatR;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;

namespace RDHub.Application.Queries.GetChargeStatus;

// Consulta o status de uma cobrança buscando na auditoria
public sealed class GetChargeStatusHandler
    : IRequestHandler<GetChargeStatusQuery, GetChargeStatusResult>
{
    private readonly IAuditRepository _auditRepository;

    public GetChargeStatusHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<GetChargeStatusResult> Handle(
        GetChargeStatusQuery query,
        CancellationToken ct)
    {

        var txId = TxId.From(query.TxId);
        var audit = await _auditRepository.GetByTxIdAsync(txId, ct)
            ?? throw new Exception("Auditoria não encontrada");

        return new GetChargeStatusResult(
            TxId: query.TxId,
            Status: audit.Status ?? string.Empty,
            Amount: audit.Amount,
            PaymentConfirmationTime: audit.PaymentConfirmationTime,
            PaymentId: audit.Id,
            Raw: "seila");
    }
}