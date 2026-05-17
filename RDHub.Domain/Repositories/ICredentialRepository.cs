using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Repositories;

// contrato para buscar e salvar credenciais do banco
public interface ICredentialRepository
{
    Task<Credential?> GetByClientIdAsync(string clientId, CancellationToken ct = default);
    Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Credential credential, CancellationToken ct = default);
    Task UpdateAsync(Credential credential, CancellationToken ct = default);
    Task DeleteAsync(Credential credential, CancellationToken ct = default);
}
