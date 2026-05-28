using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.DTOs;

public sealed record BankChargeStatusResponseDto(
    string TxId,
    string Status,
    decimal? PaidAmount,
    DateTime? PaidAt);
