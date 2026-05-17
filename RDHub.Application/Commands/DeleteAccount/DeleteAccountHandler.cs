using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.DeleteAccount;

public sealed class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Conta não encontrada");

        await _accountRepository.DeleteAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}