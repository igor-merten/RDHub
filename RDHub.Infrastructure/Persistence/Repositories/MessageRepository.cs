using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetAllByAuditoryIdAsync(Guid auditoryId, CancellationToken ct = default)
    {
        return await _context.Messages.FirstOrDefaultAsync(m => m.AuditoryId == auditoryId, ct);
    }

    public async Task AddAsync(Message message, CancellationToken ct = default)
        => await _context.Messages.AddAsync(message, ct);
}
