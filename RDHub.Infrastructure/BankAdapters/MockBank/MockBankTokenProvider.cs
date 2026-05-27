using RDHub.Domain.Aggregates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RDHub.Infrastructure.BankAdapters.MockBank;

public interface IMockBankTokenProvider
{
    Task<string> GetTokenAsync(HttpClient client, Credential credential, CancellationToken ct);
}
public class MockTokenProvider : IMockBankTokenProvider
{
    private static readonly ConcurrentDictionary<string, string> _tokenCache = new();
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<string> GetTokenAsync(HttpClient client, Credential credential, CancellationToken ct)
    {
        var clientId = credential.ClientId;

        if (_tokenCache.TryGetValue(clientId, out var cachedToken))
        {
            return cachedToken;
        }

        var clientLock = _locks.GetOrAdd(clientId, _ => new SemaphoreSlim(1, 1));

        await clientLock.WaitAsync(ct);
        try
        {
            if (_tokenCache.TryGetValue(clientId, out cachedToken))
            {
                return cachedToken;
            }

            var authRequest = new HttpRequestMessage(HttpMethod.Post, "/oauth/token");

            var formValues = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id", credential.ClientId),
                new("client_secret", credential.ClientSecret)
            };
            authRequest.Content = new FormUrlEncodedContent(formValues);

            var response = await client.SendAsync(authRequest, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            var token = doc.RootElement.GetProperty("access_token").GetString()
                        ?? throw new InvalidOperationException("Token nulo");

            _tokenCache[clientId] = token;

            return token;
        }
        finally
        {
            clientLock.Release();
        }
    }
}