using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.UpdateAccount;

public sealed class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, UpdateAccountResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateAccountResult> Handle(UpdateAccountCommand cmd, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException("Conta não encontrada");

        account.Update(cmd.CredentialId, cmd.Agency, cmd.AccountNumber, cmd.Document);

        await _accountRepository.UpdateAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new UpdateAccountResult(
            Id: account.Id,
            CredentialId: account.CredentialId);
    }
}