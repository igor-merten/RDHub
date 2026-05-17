using MediatR;

namespace RDHub.Application.Commands.UpdateAccount;

public sealed record UpdateAccountCommand(
    Guid Id,
    string Agency,
    string AccountNumber,
    string Document) : IRequest<UpdateAccountResult>;

public sealed record UpdateAccountResult(
    Guid Id,
    string Agency,
    string AccountNumber,
    string Document);