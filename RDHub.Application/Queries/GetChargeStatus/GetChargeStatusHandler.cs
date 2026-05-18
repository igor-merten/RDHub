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
        // Busca todas as auditorias pelo TxId
        var txId = TxId.From(query.TxId);
        var audits = await _auditRepository.GetByTxIdAsync(txId, ct);

        if (!audits.Any())
            throw new KeyNotFoundException("Cobrança não encontrada");

        // Verifica se existe alguma auditoria de pagamento confirmado
        var paymentConfirmed = audits.FirstOrDefault(a => a.Status == "Paid");
        if (paymentConfirmed is not null)
        {
            return new GetChargeStatusResult(
                TxId: query.TxId,
                Status: "Paid",
                PaidAmount: paymentConfirmed.PaidAmount,
                PaidAt: paymentConfirmed.PaidAt,
                PaymentId: paymentConfirmed.Id,
                Raw: "seila");
        }

        // Se não há pagamento confirmado, retorna como Open
        var invoiceAudit = audits.FirstOrDefault(a => a.Status == "Open");
        return new GetChargeStatusResult(
            TxId: query.TxId,
            Status: "Open",
            PaidAmount: null,
            PaidAt: null,
            PaymentId: null,
            Raw: null
        );
    }
}