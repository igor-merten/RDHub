using RDHub.Domain.Aggregates;
using RDHub.Domain.ValueObjects;

namespace RDHub.Domain.Repositories;

// interface para buscar e salvar cobranças Pix
public interface IPixChargeRepository
{
    Task<PixCharge?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PixCharge?> GetByTxIdAsync(TxId txId, CancellationToken ct = default);
    Task<IEnumerable<PixCharge>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(PixCharge pixCharge, CancellationToken ct = default);
    Task UpdateAsync(PixCharge pixCharge, CancellationToken ct = default);
}
