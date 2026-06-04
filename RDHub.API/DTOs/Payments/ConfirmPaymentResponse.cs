namespace RDHub.API.Contracts.Payments;

public sealed record ConfirmPaymentResponse(
    string TxId,
    bool IsPaid,
    string Status,
    DateTime? PaymentConfirmationTime);