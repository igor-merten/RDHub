using MediatR;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Commands.CreateAccount;

public sealed class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        ICredentialRepository credentialRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAccountResult> Handle(CreateAccountCommand cmd, CancellationToken ct)
    {
        // valida se a credencial existe antes de criar a conta
        if (cmd.CredentialId is not null) { 
            var credential = await _credentialRepository.GetByIdAsync(cmd.CredentialId.Value, ct)
                ?? throw new KeyNotFoundException("Credencial não encontrada");
        }

        var account = Account.Create(
            credentialId: cmd.CredentialId,
            document: cmd.Document,
            bankId: cmd.BankId,
            accountNumber: cmd.AccountNumber,
            agency: cmd.Agency);

        await _accountRepository.AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CreateAccountResult(
            Id: account.Id,
            CredentialId: account.CredentialId,
            Document: account.Document,
            BankId: account.BankId,
            AccountNumber: account.AccountNumber,
            Agency: account.Agency,
            CreatedAt: account.CreatedAt);
    }
}