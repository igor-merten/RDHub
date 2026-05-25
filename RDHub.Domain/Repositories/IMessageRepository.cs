using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct = default);
    Task<Message?> GetAllByAuditoryIdAsync(Guid auditoryId, CancellationToken ct = default);
}
