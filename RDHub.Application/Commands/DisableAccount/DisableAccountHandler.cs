using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.DisableAccount;

public sealed class DisableAccountHandler : IRequestHandler<DisableAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DisableAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accountRepository.GetActiveAndInactiveByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Conta não encontrada");

        await _accountRepository.DisableAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}