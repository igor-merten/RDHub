using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;

namespace RDHub.Infrastructure.Persistence.Repositories;

// Implementação concreta do repositório de auditoria usando EF Core
public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    public AuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Audit audit, CancellationToken ct = default)
        => await _context.Audits.AddAsync(audit, ct);

    public async Task<IEnumerable<Audit>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.Audits.Where(a => a.AccountId == userId).ToListAsync(ct);

    public async Task<Audit?> GetByTxIdAsync(TxId txId, CancellationToken ct = default)
        => await _context.Audits.Where(a => a.TxId == txId.Value).FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Audit>> GetAllOpenAsync(CancellationToken ct = default)
    => await _context.Audits
        .Where(a => a.Status == "Open")
        .ToListAsync(ct);
}
