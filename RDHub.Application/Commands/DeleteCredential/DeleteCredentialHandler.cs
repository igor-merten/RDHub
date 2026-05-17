using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.DeleteCredential;

public sealed class DeleteCredentialHandler : IRequestHandler<DeleteCredentialCommand>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCredentialHandler(ICredentialRepository credentialRepository, IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCredentialCommand cmd, CancellationToken ct)
    {
        var credential = await _credentialRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Credencial não encontrada");

        await _credentialRepository.DeleteAsync(credential, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}