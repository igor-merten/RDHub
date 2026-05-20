using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Infrastructure.Persistence.Repositories;

public class PixKeyRepository : IPixKeyRepository
{
        private readonly AppDbContext _context;

    public PixKeyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PixKey?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await _context.PixKeys.FirstOrDefaultAsync(pk => pk.Key == key, ct);
    }

    public async Task<PixKey?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PixKeys.FirstOrDefaultAsync(pk => pk.Id == id, ct);
    }

    public async Task AddAsync(PixKey pixKey, CancellationToken ct = default)
        => await _context.PixKeys.AddAsync(pixKey, ct);

    public async Task UpdateAsync(PixKey pixKey, CancellationToken ct = default)
        => _context.PixKeys.Update(pixKey);

    public async Task DeleteAsync(PixKey pixKey, CancellationToken ct = default)
        => _context.PixKeys.Remove(pixKey);
}
