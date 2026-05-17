using MediatR;
using RDHub.Domain.Repositories;

namespace RDHub.Application.Queries.GetCredentialById;

public sealed class GetCredentialByIdHandler : IRequestHandler<GetCredentialByIdQuery, GetCredentialByIdResult>
{
    private readonly ICredentialRepository _credentialRepository;

    public GetCredentialByIdHandler(ICredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public async Task<GetCredentialByIdResult> Handle(GetCredentialByIdQuery query, CancellationToken ct)
    {
        var credential = await _credentialRepository.GetByIdAsync(query.Id, ct)
            ?? throw new KeyNotFoundException("Credencial não encontrada");

        return new GetCredentialByIdResult(Id: credential.Id, ClientId: credential.ClientId);
    }
}