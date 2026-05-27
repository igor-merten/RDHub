using MediatR;

namespace RDHub.Application.Queries.GetChargeStatus;

// Consulta o status de uma cobrança sem confirmar pagamento
public sealed record GetChargeStatusQuery(
    string TxId) : IRequest<GetChargeStatusResult>;

public sealed record GetChargeStatusResult(
    string TxId,
    string Status,
    decimal? Amount,
    DateTime? PaymentConfirmationTime,
    Guid? PaymentId);