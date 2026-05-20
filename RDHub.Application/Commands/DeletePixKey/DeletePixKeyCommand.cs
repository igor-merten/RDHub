using MediatR;

namespace RDHub.Application.Commands.DeletePixKey;

public sealed record DeletePixKeyCommand(Guid Id) : IRequest;