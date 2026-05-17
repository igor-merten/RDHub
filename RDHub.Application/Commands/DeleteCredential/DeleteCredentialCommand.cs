using MediatR;

namespace RDHub.Application.Commands.DeleteCredential;

public sealed record DeleteCredentialCommand(Guid Id) : IRequest;