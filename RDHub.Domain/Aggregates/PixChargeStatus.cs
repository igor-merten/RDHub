using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

// Estados de uma cobrança Pix
public enum PixChargeStatus
{
    Active,
    Paid,
    Expired,
    Canceled
}
