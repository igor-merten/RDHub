using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.Persistence.Repositories;

// implementacao do repositório de bancos
public class BankRepository : IBankRepository
{
    private readonly AppDbContext _context;

    public BankRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Bank?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Banks.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IEnumerable<Bank>> GetAllAsync(CancellationToken ct = default)
        => await _context.Banks.ToListAsync(ct);

    public async Task AddAsync(Bank bank, CancellationToken ct = default)
        => await _context.Banks.AddAsync(bank, ct);
}