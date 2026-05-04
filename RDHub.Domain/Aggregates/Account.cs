using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates;

// Representa a conta bancária de um cliente cadastrado no HUB
public class Account : AggregateRoot<Guid>
{
    public Guid ClientId { get; private set; }
    public Guid BankId { get; private set; }
    public string Agency { get; private set; } = null!;
    public string AccountNumber { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string PixKey { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public static Account Create(
        Guid clientId,
        Guid bankId,
        string agency,
        string accountNumber,
        string cnpj,
        string pixKey)
    {
        if (clientId == Guid.Empty)
            throw new DomainException("ClientId é obrigatório");

        if (bankId == Guid.Empty)
            throw new DomainException("BankId é obrigatório");

        if (string.IsNullOrWhiteSpace(agency))
            throw new DomainException("Agência é obrigatória");

        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new DomainException("Número da conta é obrigatório");

        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ é obrigatório");

        if (string.IsNullOrWhiteSpace(pixKey))
            throw new DomainException("Chave Pix é obrigatória");

        return new Account
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            BankId = bankId,
            Agency = agency,
            AccountNumber = accountNumber,
            Cnpj = cnpj,
            PixKey = pixKey,
            CreatedAt = DateTime.UtcNow
        };
    }
}