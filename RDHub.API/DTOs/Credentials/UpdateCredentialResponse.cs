namespace RDHub.API.Contracts.Credentials;

public sealed record UpdateCredentialResponse(
    Guid Id,
    string ClientId);