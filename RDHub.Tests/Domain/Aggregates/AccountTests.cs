using RDHub.Domain.Aggregates;
using RDHub.Domain.Exceptions;
using Xunit;

namespace RDHub.Tests.Domain.Aggregates;

public class AccountTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateAccount()
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var document = "12345678900";
        var bankId = 260;
        var accountNumber = "123456";
        var agency = "0001";

        // Act
        var account = Account.Create(credentialId, document, bankId, accountNumber, agency);

        // Assert
        Assert.NotNull(account);
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(credentialId, account.CredentialId);
        Assert.Equal(document, account.Document);
        Assert.Equal(bankId, account.BankId);
        Assert.Equal(accountNumber, account.AccountNumber);
        Assert.Equal(agency, account.Agency);
        Assert.True(account.Active);
        Assert.NotEqual(DateTime.MinValue, account.CreatedAt);
    }

    [Fact]
    public void Create_WithInvalidBankId_ShouldThrowException()
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var document = "12345678900";
        var invalidBankId = 0;
        var accountNumber = "123456";
        var agency = "0001";

        // Act & Assert
        Assert.Throws<DomainException>(() => 
            Account.Create(credentialId, document, invalidBankId, accountNumber, agency));
    }

    [Theory]
    [InlineData("")]
    public void Create_WithEmptyAgency_ShouldThrowException(string invalidAgency)
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var document = "12345678900";
        var bankId = 260;
        var accountNumber = "123456";

        // Act & Assert
        Assert.Throws<DomainException>(() => 
            Account.Create(credentialId, document, bankId, accountNumber, invalidAgency));
    }

    [Theory]
    [InlineData("")]
    public void Create_WithEmptyAccountNumber_ShouldThrowException(string invalidAccountNumber)
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var document = "12345678900";
        var bankId = 260;
        var agency = "0001";

        // Act & Assert
        Assert.Throws<DomainException>(() => 
            Account.Create(credentialId, document, bankId, invalidAccountNumber, agency));
    }

    [Fact]
    public void Update_ShouldUpdateAccountData()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "12345678900", 260, "123456", "0001");
        var newCredentialId = Guid.NewGuid();
        var newAgency = "0002";
        var newAccountNumber = "654321";
        var newDocument = "98765432100";

        // Act
        account.Update(newCredentialId, newAgency, newAccountNumber, newDocument);

        // Assert
        Assert.Equal(newCredentialId, account.CredentialId);
        Assert.Equal(newAgency, account.Agency);
        Assert.Equal(newAccountNumber, account.AccountNumber);
        Assert.Equal(newDocument, account.Document);
    }

    [Fact]
    public void Deactivate_WhenAccountIsActive_ShouldDeactivate()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "12345678900", 260, "123456", "0001");
        Assert.True(account.Active);

        // Act
        account.Deactivate();

        // Assert
        Assert.False(account.Active);
    }

    [Fact]
    public void Deactivate_WhenAccountIsAlreadyInactive_ShouldThrowException()
    {
        // Arrange
        var account = Account.Create(Guid.NewGuid(), "12345678900", 260, "123456", "0001");
        account.Deactivate();

        // Act & Assert
        Assert.Throws<DomainException>(() => account.Deactivate());
    }
}
