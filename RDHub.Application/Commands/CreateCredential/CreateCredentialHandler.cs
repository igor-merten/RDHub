using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Exceptions;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.CreateCredential;

public sealed class CreateCredentialHandler : IRequestHandler<CreateCredentialCommand, CreateCredentialResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCredentialHandler(ICredentialRepository credentialRepository, IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCredentialResult> Handle(CreateCredentialCommand cmd, CancellationToken ct)
    {
        var existing = await _credentialRepository.GetByClientIdAsync(cmd.ClientId, ct);
        if (existing is not null)
            throw new DomainException("Já existe uma credencial com esse ClientId");

        var credential = Credential.Create(
            clientId: cmd.ClientId,
            clientSecret: cmd.ClientSecret,
            certificate: cmd.Certificate,
            certificatePassword: cmd.CertificatePassword);

        await _credentialRepository.AddAsync(credential, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateCredentialResult(Id: credential.Id, ClientId: credential.ClientId);
    }
}