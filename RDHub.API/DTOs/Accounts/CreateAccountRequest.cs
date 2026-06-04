namespace RDHub.API.Contracts.Accounts;

public sealed record CreateAccountRequest(
    Guid? CredentialId,
    string Document,
    int BankId,
    string AccountNumber,
    string Agency);