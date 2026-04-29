using RDHub.Application.DTOs;
using RDHub.Application.Interfaces;

namespace RDHub.Infrastructure.BankAdapters.Abstractions;

// Classe base para todos os adapters de banco
public abstract class BaseBankPixAdapter : IBankPixAdapter
{
    public abstract string BankId { get; }
    public abstract Task<BankChargeResponse> CreateChargeAsync(BankChargeRequest request, CancellationToken ct = default);
    public abstract Task<BankChargeStatusResponse> GetChargeStatusAsync(string txId, CancellationToken ct = default);
}