
using RDHub.Domain.Aggregates;
using System.Text;

namespace RDHub.Domain.Repositories;

// interface para buscar e salvar faturas
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    Task UpdateAsync(Invoice invoice, CancellationToken ct = default);
}
