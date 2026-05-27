using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Infrastructure.BankAdapters.MockBank;

public class MockBankOptions
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}