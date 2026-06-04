using MediatR;

namespace RDHub.Application.Commands.EnableAccount;

public sealed record EnableAccountCommand(Guid Id) : IRequest;