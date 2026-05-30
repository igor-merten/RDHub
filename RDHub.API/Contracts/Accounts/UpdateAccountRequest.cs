namespace RDHub.API.Contracts.Accounts;

public sealed record UpdateAccountRequest(Guid? CredentialId, string? Agency, string? AccountNumber, string? Document);