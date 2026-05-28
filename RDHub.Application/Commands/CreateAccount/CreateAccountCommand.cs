using MediatR;

namespace RDHub.Application.Commands.CreateAccount;

public sealed record CreateAccountCommand(
    Guid? CredentialId,
    string Document,
    int BankId,
    string AccountNumber,
    string Agency) : IRequest<CreateAccountResult>;

public sealed record CreateAccountResult(
    Guid Id,
    Guid? CredentialId,
    string? Document,
    int BankId,
    string? AccountNumber,
    string? Agency,
    DateTime CreatedAt);