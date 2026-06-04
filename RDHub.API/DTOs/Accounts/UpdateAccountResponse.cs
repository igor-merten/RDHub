namespace RDHub.API.Contracts.Accounts;

public sealed record UpdateAccountResponse(
    Guid Id,
    Guid? CredentialId);