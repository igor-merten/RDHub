using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.Persistence.Repositories;

// implementacao do repositório de auditoria 
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
        => await _context.Audits.Where(a => a.UserId == userId).ToListAsync(ct);
}