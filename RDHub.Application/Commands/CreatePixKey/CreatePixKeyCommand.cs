using MediatR;

namespace RDHub.Application.Commands.CreatePixKey;

public sealed record CreatePixKeyCommand(
    string Key,
    Guid AccountId) : IRequest<CreatePixKeyResult>;

public sealed record CreatePixKeyResult(
    Guid Id,
    string Key,
    Guid AccountId);