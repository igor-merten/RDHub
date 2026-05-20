using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Queries.GetPixKeyById;

public sealed record GetPixKeyByIdQuery(Guid Id) : IRequest<GetPixKeyByIdResult>;

public sealed record GetPixKeyByIdResult(Guid Id, string Key, Guid AccountId);