using MediatR;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Queries.GetAccountById;

public sealed class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, GetAccountByIdResult>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountByIdResult> Handle(GetAccountByIdQuery query, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(query.Id, ct)
            ?? throw new KeyNotFoundException("Conta não encontrada");

        return new GetAccountByIdResult(
            Id: account.Id,
            CredentialId: account.CredentialId,
            Document: account.Document,
            BankId: account.BankId,
            AccountNumber: account.AccountNumber,
            Agency: account.Agency,
            CreatedAt: account.CreatedAt);
    }
}