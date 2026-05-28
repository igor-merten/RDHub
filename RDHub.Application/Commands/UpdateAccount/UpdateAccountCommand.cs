using MediatR;

namespace RDHub.Application.Commands.UpdateAccount;

public sealed record UpdateAccountCommand(
    Guid Id,
    Guid? CredentialId) : IRequest<UpdateAccountResult>;

public sealed record UpdateAccountResult(
    Guid Id,
    Guid? CredentialId);