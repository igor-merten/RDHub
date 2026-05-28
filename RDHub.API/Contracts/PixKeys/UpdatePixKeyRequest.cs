namespace RDHub.API.Contracts.PixKeys;

public sealed record UpdatePixKeyRequest(
    string Key,
    Guid AccountId);