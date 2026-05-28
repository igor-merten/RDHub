namespace RDHub.API.Contracts.Payments;

public sealed record CreateCobRequest(
    Guid InvoiceId,
    decimal Amount,
    string PixKey,
    int ExpireInSeconds,
    string PayerMessage);