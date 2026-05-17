using MediatR;

namespace RDHub.Application.Queries.GetAccountById;

public sealed record GetAccountByIdQuery(Guid Id) : IRequest<GetAccountByIdResult>;

public sealed record GetAccountByIdResult(
    Guid Id,
    Guid CredentialId,
    string Document,
    int BankId,
    string AccountNumber,
    string Agency,
    DateTime CreatedAt);