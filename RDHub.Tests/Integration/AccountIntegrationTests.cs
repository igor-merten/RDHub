using System.Net;
using RDHub.API.Contracts.Accounts;
using RDHub.API.DTOs.Accounts;
using RDHub.Infrastructure.Persistence;

namespace RDHub.Tests.Integration;

/// <summary>
/// Testes de integração para o endpoint de Accounts.
/// </summary>
public class AccountIntegrationTestsV2 : IntegrationTestBase
{
    private const string ApiVersion = "v1";
    private const string BaseUri = $"/api/{ApiVersion}/accounts";

    [Fact]
    public async Task CreateAccount_WithValidData_ReturnsCreatedStatusCode()
    {
        // Arrange
        var request = new CreateAccountRequest(
            CredentialId: null,
            Document: "12345678901234",
            BankId: 1,
            AccountNumber: "123456",
            Agency: "0001"
        );

        // Act
        var response = await PostAsync(BaseUri, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_WithValidData_PersistsToDatabase()
    {
        // Arrange
        var document = "12345678901234";
        var request = new CreateAccountRequest(
            CredentialId: null,
            Document: document,
            BankId: 1,
            AccountNumber: "123456",
            Agency: "0001"
        );

        // Act
        var response = await PostAsync<CreateAccountResponse>(BaseUri, request);
        Assert.NotNull(response);

        // Assert
        var db = GetDbContext();
        var account = await db.Accounts.FindAsync(response.Id);
        Assert.NotNull(account);
        Assert.Equal(document, account.Document);
        Assert.Equal(1, account.BankId);
        Assert.True(account.Active);
    }

    [Fact]
    public async Task GetAccountById_WithValidId_ReturnsAccountData()
    {
        // Arrange
        var createRequest = new CreateAccountRequest(
            CredentialId: null,
            Document: "12345678901234",
            BankId: 1,
            AccountNumber: "123456",
            Agency: "0001"
        );

        var createResponse = await PostAsync<CreateAccountResponse>(BaseUri, createRequest);
        Assert.NotNull(createResponse);

        // Act
        var response = await GetAsync<GetAccountByIdResponse>($"{BaseUri}/{createResponse.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(createResponse.Id, response.Id);
    }

    [Fact]
    public async Task GetAccountById_WithInvalidId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await GetAsync($"{BaseUri}/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
