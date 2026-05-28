namespace RDHub.API.Contracts.Accounts;

public sealed record CreateAccountResponse(
    Guid Id,
    Guid? CredentialId,
    string? Document,
    int BankId,
    string? AccountNumber,
    string? Agency,
    DateTime CreatedAt);