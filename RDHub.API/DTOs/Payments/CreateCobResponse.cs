namespace RDHub.API.Contracts.Payments;

public sealed record CreateCobResponse(
    string TxId,
    string Status,
    string Emv,
    string PixLink);