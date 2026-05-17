using MediatR;

namespace RDHub.Application.Commands.UpdateCredential;

public sealed record UpdateCredentialCommand(
    Guid Id,
    string ClientSecret,
    string Certificate,
    string CertificatePassword) : IRequest<UpdateCredentialResult>;

public sealed record UpdateCredentialResult(Guid Id, string ClientId);