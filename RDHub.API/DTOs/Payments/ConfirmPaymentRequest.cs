namespace RDHub.API.Contracts.Payments;

public sealed record ConfirmPaymentRequest(
    string TxId,
    string Status,
    decimal PaidAmount);