using MediatR;

namespace RDHub.Application.Commands.UpdateAccount;

public sealed record UpdateAccountCommand(
    Guid Id,
    string? Agency,
    string? AccountNumber,
    string? Document,
    Guid? CredentialId) : IRequest<UpdateAccountResult>;

public sealed record UpdateAccountResult(
    Guid Id,
    Guid? CredentialId);