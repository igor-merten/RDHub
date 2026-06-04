using MediatR;

namespace RDHub.Application.Commands.DisableAccount;

public sealed record DisableAccountCommand(Guid Id) : IRequest;