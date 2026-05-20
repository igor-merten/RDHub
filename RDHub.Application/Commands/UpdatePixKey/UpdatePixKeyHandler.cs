using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.UpdatePixKey;

public sealed class UpdatePixKeyHandler : IRequestHandler<UpdatePixKeyCommand, UpdatePixKeyResult>
{
    private readonly IPixKeyRepository _pixKeyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePixKeyHandler(IPixKeyRepository pixKeyRepository, IUnitOfWork unitOfWork)
    {
        _pixKeyRepository = pixKeyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePixKeyResult> Handle(UpdatePixKeyCommand cmd, CancellationToken ct)
    {
        
        var pixKey = await _pixKeyRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Chave Pix não encontrada");

        pixKey.Update(cmd.Key, cmd.AccountId);

        await _pixKeyRepository.UpdateAsync(pixKey, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new UpdatePixKeyResult(Id: pixKey.Id, Key: pixKey.Key, AccountId: pixKey.AccountId);
    }
}
