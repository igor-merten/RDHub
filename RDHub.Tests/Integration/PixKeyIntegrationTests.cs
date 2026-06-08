using System.Net;
using RDHub.API.Contracts.PixKeys;
using RDHub.API.Contracts.Accounts;
using RDHub.Infrastructure.Persistence;

namespace RDHub.Tests.Integration;

/// <summary>
/// Testes de integração para o endpoint de PixKeys.
/// </summary>
public class PixKeyIntegrationTestsV2 : IntegrationTestBase
{
    private const string ApiVersion = "v1";
    private const string AccountsUri = $"/api/{ApiVersion}/accounts";
    private const string PixKeysUri = $"/api/{ApiVersion}/pixkeys";

    private async Task<Guid> CreateAccountAsync()
    {
        var request = new CreateAccountRequest(
            CredentialId: null,
            Document: "12345678901234",
            BankId: 1,
            AccountNumber: "123456",
            Agency: "0001"
        );

        var response = await PostAsync<CreateAccountResponse>(AccountsUri, request);
        Assert.NotNull(response);
        return response.Id;
    }

    [Fact]
    public async Task CreatePixKey_WithValidData_ReturnsCreatedStatusCode()
    {
        // Arrange
        var accountId = await CreateAccountAsync();

        var request = new CreatePixKeyRequest(
            Key: "joao@example.com",
            AccountId: accountId
        );

        // Act
        var response = await PostAsync(PixKeysUri, request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPixKeyById_WithInvalidId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await GetAsync($"{PixKeysUri}/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
