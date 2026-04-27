using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

public enum PixChargeStatus
{
    Active,
    Paid,
    Expired,
    Canceled
}
