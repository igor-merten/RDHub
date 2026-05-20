using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Exceptions;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.CreatePixKey;

public sealed class CreatePixKeyHandler : IRequestHandler<CreatePixKeyCommand, CreatePixKeyResult>
{
    private readonly IPixKeyRepository _pixKeyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePixKeyHandler(IPixKeyRepository pixKeyRepository, IUnitOfWork unitOfWork)
    {
        _pixKeyRepository = pixKeyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePixKeyResult> Handle(CreatePixKeyCommand cmd, CancellationToken ct)
    {
        var existing = await _pixKeyRepository.GetByKeyAsync(cmd.Key, ct);
        if (existing is not null)
            throw new DomainException("Já existe uma chave Pix com esse valor");

        var pixKey = PixKey.Create(
            key: cmd.Key,
            accountId: cmd.AccountId);

        await _pixKeyRepository.AddAsync(pixKey, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreatePixKeyResult(Id: pixKey.Id, Key: pixKey.Key, AccountId: pixKey.AccountId);
    }
}