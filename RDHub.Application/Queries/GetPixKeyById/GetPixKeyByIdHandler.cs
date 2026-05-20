using MediatR;
using RDHub.Application.Queries.GetCredentialById;
using RDHub.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Queries.GetPixKeyById;

public sealed class GetPixKeyByIdHandler : IRequestHandler<GetPixKeyByIdQuery, GetPixKeyByIdResult>
{
    private readonly IPixKeyRepository _pixKeyRepository;

    public GetPixKeyByIdHandler(IPixKeyRepository pixKeyRepository)
    {
        _pixKeyRepository = pixKeyRepository;
    }

    public async Task<GetPixKeyByIdResult> Handle(GetPixKeyByIdQuery query, CancellationToken ct)
    {
        var pixKey = await _pixKeyRepository.GetByIdAsync(query.Id, ct)
            ?? throw new KeyNotFoundException("Chave Pix não encontrada");

        return new GetPixKeyByIdResult(Id: pixKey.Id, Key: pixKey.Key, AccountId: pixKey.AccountId);
    }
}