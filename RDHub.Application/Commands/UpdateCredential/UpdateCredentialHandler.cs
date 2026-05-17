using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.UpdateCredential;

public sealed class UpdateCredentialHandler : IRequestHandler<UpdateCredentialCommand, UpdateCredentialResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCredentialHandler(ICredentialRepository credentialRepository, IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCredentialResult> Handle(UpdateCredentialCommand cmd, CancellationToken ct)
    {
        var credential = await _credentialRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Credencial não encontrada");

        credential.Update(cmd.ClientSecret, cmd.Certificate, cmd.CertificatePassword);

        await _credentialRepository.UpdateAsync(credential, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new UpdateCredentialResult(Id: credential.Id, ClientId: credential.ClientId);
    }
}