using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.DTOs;

public sealed record BankChargeRequest(
    string TxId,
    PixChargeType Type,
    decimal Amount,
    string PixKey,
    DateOnly? DueDate = null,
    int? ExpiresInSeconds = null,
    string? PayerMessage = null
    );

public sealed record BankChargeResponse(
    string TxId,
    string Status,
    string Emv);

public sealed record BankChargeStatusResponse(
    string TxId,
    string Status,
    decimal? PaidAmount,
    DateTime? PaidAt);

public enum PixChargeType { Cob, CobV }
