using MediatR;

namespace RDHub.Application.Commands.CreateCredential;

public sealed record CreateCredentialCommand(
    string ClientId,
    string ClientSecret,
    string Certificate,
    string CertificatePassword) : IRequest<CreateCredentialResult>;

public sealed record CreateCredentialResult(
    Guid Id,
    string ClientId);