namespace RDHub.API.Contracts.PixKeys;

public sealed record CreatePixKeyResponse(
    Guid Id,
    string Key,
    Guid AccountId);