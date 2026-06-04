namespace RDHub.API.Contracts.PixKeys;

public sealed record GetPixKeyByIdResponse(
    Guid Id,
    string Key,
    Guid AccountId);