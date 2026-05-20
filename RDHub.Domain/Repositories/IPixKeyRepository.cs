using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Repositories;

public interface IPixKeyRepository
{
    Task<PixKey?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<PixKey?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(PixKey pixKey, CancellationToken ct = default);
    Task UpdateAsync(PixKey pixKey, CancellationToken ct = default);
    Task DeleteAsync(PixKey pixKey, CancellationToken ct = default);
}
