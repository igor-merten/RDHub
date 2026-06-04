namespace RDHub.API.Contracts.Credentials;

public sealed record CreateCredentialResponse(
    Guid Id,
    string ClientId);