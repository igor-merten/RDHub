using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Commands.UpdatePixKey;

public sealed record UpdatePixKeyCommand(
    Guid Id,
    string Key,
    Guid AccountId) : IRequest<UpdatePixKeyResult>;

public sealed record UpdatePixKeyResult(Guid Id, string Key, Guid AccountId);
