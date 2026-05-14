using RDHub.Domain.Exceptions;

namespace RDHub.Domain.Aggregates
{
    // Representa as chaves PIX de uma Account no HUB
    public class PixKey : AggregateRoot<Guid>
    {
        public string Key { get; private set; } = null!;
        public Guid AccountId { get; private set; }

        private PixKey() { }

        public static PixKey Create(string key, Guid accountId)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new DomainException("A chave Pix é obrigatória");
            if (accountId == Guid.Empty) throw new DomainException("AccountId inválido");

            return new PixKey
            {
                Id = Guid.NewGuid(),
                Key = key,
                AccountId = accountId
            };
        }
    }
}
