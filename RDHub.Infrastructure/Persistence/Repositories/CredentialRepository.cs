using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.Persistence.Repositories;

// Implementação concreta do repositório de credenciais usando EF Core
public class CredentialRepository : ICredentialRepository
{
    private readonly AppDbContext _context;

    public CredentialRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Credential?> GetByClientIdAsync(string clientId, CancellationToken ct = default)
        => await _context.Credentials.FirstOrDefaultAsync(s => s.ClientId == clientId, ct);
    //public async Task<Credential?> GetByBankIdAsync(int bankId, CancellationToken ct = default)
    //=> await _context.Credentials.FirstOrDefaultAsync(c => c.BankId == bankId, ct);

    public async Task AddAsync(Credential credential, CancellationToken ct = default)
        => await _context.Credentials.AddAsync(credential, ct);
    public async Task UpdateAsync(Credential credential, CancellationToken ct = default)
        => _context.Credentials.Update(credential);
}