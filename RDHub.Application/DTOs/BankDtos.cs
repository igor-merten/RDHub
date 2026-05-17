using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.DTOs;

public sealed record BankChargeRequest(
    string TxId,
    decimal Amount);

public sealed record BankChargeResponse(
    string TxId,
    string Status,
    string Emv);

public sealed record BankChargeStatusResponse(
    string TxId,
    string Status,
    decimal? PaidAmount,
    DateTime? PaidAt);