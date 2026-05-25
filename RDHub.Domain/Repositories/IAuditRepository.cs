using RDHub.Domain.Aggregates;
using RDHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Repositories;

// contrato para registrat e consultar auditorias
public interface IAuditRepository
{
    Task AddAsync(Audit audit, CancellationToken ct = default); 
    Task<Audit?> GetByTxIdAsync(TxId txId, CancellationToken ct = default);
    Task<IEnumerable<Audit>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Audit>> GetAllOpenAsync(CancellationToken ct = default);
}
