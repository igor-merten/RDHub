using MediatR;

namespace RDHub.Application.Commands.DeleteAccount;

public sealed record DeleteAccountCommand(Guid Id) : IRequest;