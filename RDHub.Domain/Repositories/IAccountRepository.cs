using RDHub.Domain.Aggregates;

namespace RDHub.Domain.Repositories;

// Contrato para buscar e salvar contas bancárias
public interface IAccountRepository
{
    Task<Account?> GetByPixKeyAsync(string pixKey, CancellationToken ct = default);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Account?> GetActiveAndInactiveByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    Task UpdateAsync(Account account, CancellationToken ct = default);
    Task DisableAsync(Account account, CancellationToken ct = default);
    Task EnableAsync(Account account, CancellationToken ct = default);
}