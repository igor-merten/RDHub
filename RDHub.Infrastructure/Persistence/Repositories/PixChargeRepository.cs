using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;

namespace RDHub.Infrastructure.Persistence.Repositories;

// implementacao concreta do repositório de cobranças Pix
public class PixChargeRepository : IPixChargeRepository
{
    private readonly AppDbContext _context;

    public PixChargeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PixCharge?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.PixCharges.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PixCharge?> GetByTxIdAsync(TxId txId, CancellationToken ct = default)
        => await _context.PixCharges.FirstOrDefaultAsync(p => p.TxId.Value == txId.Value, ct);

    public async Task AddAsync(PixCharge pixCharge, CancellationToken ct = default)
        => await _context.PixCharges.AddAsync(pixCharge, ct);

    public async Task UpdateAsync(PixCharge pixCharge, CancellationToken ct = default)
        => _context.PixCharges.Update(pixCharge);

    public async Task<IEnumerable<PixCharge>> GetAllActiveAsync(CancellationToken ct = default)
    => await _context.PixCharges
        .Where(p => p.Status == PixChargeStatus.Active)
        .ToListAsync(ct);
}