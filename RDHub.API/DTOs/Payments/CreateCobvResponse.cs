namespace RDHub.API.Contracts.Payments;

public sealed record CreateCobvResponse(
    string TxId,
    string Status,
    string Emv,
    string PixLink);