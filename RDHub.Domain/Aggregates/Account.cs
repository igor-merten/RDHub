using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates;

// Representa a conta bancária de um cliente cadastrado no HUB
public class Account : AggregateRoot<Guid>
{
    public Guid? CredentialId { get; private set; }
    public int BankId { get; private set; }
    public string? Agency { get; private set; } = null!;
    public string? AccountNumber { get; private set; } = null!;
    public string? Document { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public bool Active { get; private set; }


    // Propriedades de Navegação (Para o EF Core)
    public virtual Credential Credential { get; private set; } = null!;
    public virtual ICollection<PixKey> PixKeys { get; private set; } = new List<PixKey>();

    private Account() { }

    public static Account Create(
        Guid? credentialId,
        string document,
        int bankId,
        string accountNumber,
        string agency)
    {
        if (bankId <= 0)
            throw new DomainException("BankId é obrigatório");

        if (string.IsNullOrWhiteSpace(agency))
            throw new DomainException("Agência é obrigatória");

        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new DomainException("Número da conta é obrigatório");

        if (string.IsNullOrWhiteSpace(document))
            throw new DomainException("Documento é obrigatório");

        return new Account
        {
            Id = Guid.NewGuid(),
            CredentialId = credentialId,
            Document = document,
            BankId = bankId,
            AccountNumber = accountNumber,
            Agency = agency,
            CreatedAt = DateTime.UtcNow,
            Active = true
        };
    }

    public void Update(Guid? credentialId, string? agency, string? accountNumber, string? document)
    {
        CredentialId = credentialId;
        Agency = agency ?? Agency;
        AccountNumber = accountNumber ?? AccountNumber;
        Document = document ?? Document;
    }

    public void Deactivate()
    {
        if (!Active)
            throw new DomainException("Conta já está inativa");

        Active = false;
    }

    public void Activate()
    {
        if (Active)
            throw new DomainException("Conta já está ativa");

        Active = true;
    }
}