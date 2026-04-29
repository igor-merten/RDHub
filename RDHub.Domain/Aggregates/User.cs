using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates;

// Representa um usuário cadastrado no HUB
public class User : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string PixKey { get; private set; } = null!;
    public string Account { get; private set; } = null!;
    public string Agency { get; private set; } = null!;
    public string BankId { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(
        string name,
        string cnpj,
        string pixKey,
        string account,
        string agency,
        string bankId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ é obrigatório");

        if (string.IsNullOrWhiteSpace(pixKey))
            throw new DomainException("Chave Pix é obrigatória");

        if (string.IsNullOrWhiteSpace(bankId))
            throw new DomainException("Banco é obrigatório");

        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Cnpj = cnpj,
            PixKey = pixKey,
            Account = account,
            Agency = agency,
            BankId = bankId,
            CreatedAt = DateTime.UtcNow
        };
    }
}