namespace RDHub.API.Contracts.PixKeys;

public sealed record UpdatePixKeyResponse(
    Guid Id,
    string Key,
    Guid AccountId);