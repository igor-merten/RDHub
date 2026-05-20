using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.DeletePixKey;

public sealed class DeletePixKeyHandler : IRequestHandler<DeletePixKeyCommand>
{
    private readonly IPixKeyRepository _pixKeyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePixKeyHandler(IPixKeyRepository pixKeyRepository, IUnitOfWork unitOfWork)
    {
        _pixKeyRepository = pixKeyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePixKeyCommand cmd, CancellationToken ct)
    {
        var pixKey = await _pixKeyRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Chave Pix não encontrada");

        await _pixKeyRepository.DeleteAsync(pixKey, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}