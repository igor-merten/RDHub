namespace RDHub.API.Contracts.Accounts;

public sealed record GetAccountByIdResponse(
    Guid Id,
    Guid? CredentialId,
    string? Document,
    int BankId,
    string? AccountNumber,
    string? Agency,
    DateTime CreatedAt);