using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

public interface IBankPixAdapter
{
    string BankId { get; }
    Task<BankChargeResponse> CreateChargeAsync(BankChargeRequest request, CancellationToken ct = default);
    Task<BankChargeStatusResponse> GetChargeStatusAsync(string txId, CancellationToken ct = default);

    public sealed record BankChargeRequest(
    string TxId,
    decimal Amount,
    string PixKey);

    public sealed record BankChargeResponse(
        string TxId,
        string Status,
        string Emv);

    public sealed record BankChargeStatusResponse(
        string TxId,
        string Status,
        decimal? PaidAmount,
        DateTime? PaidAt);
}
