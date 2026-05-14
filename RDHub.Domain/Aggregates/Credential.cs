using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates;

// Representa as credenciais de autenticação de um banco no HUB
public class Credential : AggregateRoot<Guid>
{
    public string ClientId { get; private set; } = null!;
    public string ClientSecret { get; private set; } = null!;
    public string Certificate { get; private set; } = null!;
    public string CertificatePassword { get; private set; } = null!;

    private Credential() { }

    public static Credential Create(
        string clientId,
        string clientSecret,
        string certificate,
        string certificatePassword)
    {

        if (string.IsNullOrWhiteSpace(clientId))
            throw new DomainException("ClientId é obrigatório");

        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new DomainException("ClientSecret é obrigatório");

        return new Credential
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            ClientSecret = clientSecret,
            Certificate = certificate,
            CertificatePassword = certificatePassword,
        };
    }
}