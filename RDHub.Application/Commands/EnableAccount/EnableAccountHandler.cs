using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.EnableAccount;

public sealed class EnableAccountHandler : IRequestHandler<EnableAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnableAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EnableAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accountRepository.GetActiveAndInactiveByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Conta não encontrada");

        await _accountRepository.EnableAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}