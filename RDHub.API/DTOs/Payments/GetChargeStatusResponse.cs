namespace RDHub.API.Contracts.Payments;

public sealed record GetChargeStatusResponse(
    string TxId,
    string Status,
    decimal? Amount,
    DateTime? PaidAt,
    Guid? PaymentId,
    object Raw);