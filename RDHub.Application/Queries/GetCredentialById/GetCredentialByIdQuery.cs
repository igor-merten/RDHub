using MediatR;

namespace RDHub.Application.Queries.GetCredentialById;

public sealed record GetCredentialByIdQuery(Guid Id) : IRequest<GetCredentialByIdResult>;

public sealed record GetCredentialByIdResult(Guid Id, string ClientId);