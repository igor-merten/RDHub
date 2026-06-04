namespace RDHub.API.Contracts.Credentials;

public sealed record GetCredentialByIdResponse(
    Guid Id,
    string ClientId);