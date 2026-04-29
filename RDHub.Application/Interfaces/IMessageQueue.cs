using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

public interface IMessageQueue
{
    Task PublishAsync<T>(string queue, T message, CancellationToken ct = default);
}
