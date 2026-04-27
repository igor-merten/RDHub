using RDHub.Domain.Aggregates;
using RDHub.Domain.ValueObjects;

namespace RDHub.Domain.Repositories;

public interface IPixChargeRepository
{
    Task<PixCharge?> GetByIdAsync(TxId txId, CancellationToken ct = default);
    Task AddAsync(PixCharge pixCharge, CancellationToken ct = default);
    Task UpdateAsync(PixCharge pixCharge, CancellationToken ct = default);
}
