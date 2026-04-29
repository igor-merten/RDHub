using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

// garante que várias operações no banco aconteçam juntas ou não aconteçam nenhuma
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
