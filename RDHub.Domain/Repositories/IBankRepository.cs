using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Repositories;

// contrato para buscar e salvar bancos
public interface IBankRepository
{
    Task<Bank?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Bank>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Bank bank, CancellationToken ct = default);
}
