namespace RDHub.API.Contracts.Payments;

public sealed record CreateCobvRequest(
    Guid InvoiceId,
    decimal Amount,
    string PixKey,
    DateOnly DueDate,
    string? PayerMessage);