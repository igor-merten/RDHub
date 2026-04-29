using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

// retorna o adapter correto baseado no BankId
public interface IBankAdapterFactory
{
    IBankPixAdapter Get(string bankId);
}
