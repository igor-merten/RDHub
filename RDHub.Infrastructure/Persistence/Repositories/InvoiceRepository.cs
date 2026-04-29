using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.Persistence.Repositories;

// implementação do repositório de faturas 
public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
        => await _context.Invoices.AddAsync(invoice, ct);

    public async Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
        => _context.Invoices.Update(invoice);
}