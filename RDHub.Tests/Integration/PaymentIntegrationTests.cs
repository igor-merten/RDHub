using System.Net;
using RDHub.API.Contracts.Payments;
using RDHub.API.Contracts.Accounts;
using RDHub.Infrastructure.Persistence;

namespace RDHub.Tests.Integration;

/// <summary>
/// Testes de integração para o endpoint de Payments/Charges.
/// </summary>
public class PaymentIntegrationTestsV2 : IntegrationTestBase
{
    private const string ApiVersion = "v1";
    private const string AccountsUri = $"/api/{ApiVersion}/accounts";
    private const string PaymentsUri = $"/api/{ApiVersion}/payments";

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
    public async Task GetChargeStatus_WithInvalidChargeId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await GetAsync($"{PaymentsUri}/{invalidId}/status");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
