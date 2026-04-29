using RDHub.Application.Interfaces;

namespace RDHub.Infrastructure.Persistence;

// garante que todas as operações do banco sejam salvas juntas
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}