namespace RDHub.API.Contracts.PixKeys;

public sealed record CreatePixKeyRequest(
    string Key,
    Guid AccountId);