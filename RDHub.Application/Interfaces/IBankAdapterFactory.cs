using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

public interface IBankAdapterFactory
{
    IBankPixAdapter Get(string bankId);
}
