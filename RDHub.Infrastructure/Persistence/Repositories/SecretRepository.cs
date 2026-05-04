using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.Persistence.Repositories;

// Implementação concreta do repositório de credenciais usando EF Core
public class SecretRepository : ISecretRepository
{
    private readonly AppDbContext _context;

    public SecretRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Secret?> GetByClientIdAsync(string clientId, CancellationToken ct = default)
        => await _context.Secrets.FirstOrDefaultAsync(s => s.ClientId == clientId, ct);

    public async Task AddAsync(Secret secret, CancellationToken ct = default)
        => await _context.Secrets.AddAsync(secret, ct);

    public async Task UpdateAsync(Secret secret, CancellationToken ct = default)
        => _context.Secrets.Update(secret);
}