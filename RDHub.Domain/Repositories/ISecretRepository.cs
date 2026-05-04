using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Repositories;

// contrato para buscar e salvar credenciais do banco
public interface ISecretRepository
{
    Task<Secret?> GetByClientIdAsync(string clientId, CancellationToken ct = default);
    Task AddAsync(Secret secret, CancellationToken ct = default);
    Task UpdateAsync(Secret secret, CancellationToken ct = default);
}
