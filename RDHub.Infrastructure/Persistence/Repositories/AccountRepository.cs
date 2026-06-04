using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.Persistence.Repositories;

// implementação do repositório de contas bancárias 
public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default)

    => await _context.Accounts
        .Include(a => a.Credential)
        .Include(a => a.PixKeys)
        .FirstOrDefaultAsync(a => a.Id == id && a.Active, ct);

    public async Task<Account?> GetActiveAndInactiveByIdAsync(Guid id, CancellationToken ct = default)

    => await _context.Accounts
        .Include(a => a.Credential)
        .Include(a => a.PixKeys)
        .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<Account?> GetByPixKeyAsync(string pixKey, CancellationToken ct = default)
    => await _context.Accounts
        .Include(a => a.Credential)
        .Include(a => a.PixKeys)
        .FirstOrDefaultAsync(a => a.Active && a.PixKeys.Any(pk => pk.Key == pixKey), ct);

    public async Task AddAsync(Account account, CancellationToken ct = default)
        => await _context.Accounts.AddAsync(account, ct);

    public async Task UpdateAsync(Account account, CancellationToken ct = default)
        => _context.Accounts.Update(account);

    public async Task DisableAsync(Account account, CancellationToken ct = default)
    {
        account.Deactivate();
        _context.Accounts.Update(account);
    }

    public async Task EnableAsync(Account account, CancellationToken ct = default)
    {
        account.Activate();
        _context.Accounts.Update(account);
    }
}