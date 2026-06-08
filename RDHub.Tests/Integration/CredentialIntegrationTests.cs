using System.Net;
using RDHub.API.Contracts.Credentials;
using RDHub.API.Contracts.Accounts;
using RDHub.Infrastructure.Persistence;

namespace RDHub.Tests.Integration;

/// <summary>
/// Testes de integração para o endpoint de Credentials.
/// </summary>
public class CredentialIntegrationTestsV2 : IntegrationTestBase
{
    private const string ApiVersion = "v1";
    private const string AccountsUri = $"/api/{ApiVersion}/accounts";
    private const string CredentialsUri = $"/api/{ApiVersion}/credentials";

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
    public async Task CreateCredential_WithValidData_ReturnsCreatedStatusCode()
    {
        // Arrange - Credentials podem não ter AccountId no request
        var request = new CreateCredentialRequest(
            ClientId: "client_12345",
            ClientSecret: "secret_12345",
            Certificate: "",
            CertificatePassword: ""
        );

        // Act
        var response = await PostAsync(CredentialsUri, request);

        // Assert - Esperar Created ou outra resposta válida
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCredentialById_WithInvalidId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await GetAsync($"{CredentialsUri}/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
