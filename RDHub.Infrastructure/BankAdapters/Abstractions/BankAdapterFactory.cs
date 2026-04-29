using RDHub.Application.Interfaces;

namespace RDHub.Infrastructure.BankAdapters.Abstractions;

// retorna o adapter correto baseado no BankId
public class BankAdapterFactory : IBankAdapterFactory
{
    private readonly Dictionary<string, IBankPixAdapter> _adapters;

    public BankAdapterFactory(IEnumerable<IBankPixAdapter> adapters)
    {
        _adapters = adapters.ToDictionary(
            a => a.BankId,
            StringComparer.OrdinalIgnoreCase);
    }

    public IBankPixAdapter Get(string bankId)
    {
        if (!_adapters.TryGetValue(bankId, out var adapter))
            throw new KeyNotFoundException($"Adapter não encontrado para o banco: {bankId}");

        return adapter;
    }
}