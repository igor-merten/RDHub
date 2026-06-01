using Moq;
using RDHub.Application.Commands.CreateAccount;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Domain.Repositories;
using Xunit;

namespace RDHub.Tests.Application.Commands;

public class CreateAccountHandlerTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<ICredentialRepository> _mockCredentialRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateAccountHandler _handler;

    public CreateAccountHandlerTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockCredentialRepository = new Mock<ICredentialRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new CreateAccountHandler(
            _mockAccountRepository.Object,
            _mockCredentialRepository.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateAccount()
    {
        // Arrange
        var command = new CreateAccountCommand(
            CredentialId: null,
            Document: "12345678900",
            BankId: 260,
            AccountNumber: "123456",
            Agency: "0001");

        _mockAccountRepository.Setup(x => x.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.Document, result.Document);
        Assert.Equal(command.BankId, result.BankId);
        Assert.Equal(command.AccountNumber, result.AccountNumber);
        Assert.Equal(command.Agency, result.Agency);
        
        _mockAccountRepository.Verify(
            x => x.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCredentialId_ShouldVerifyCredentialExists()
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var credential = Credential.Create(
            clientId: "test-client",
            clientSecret: "test-secret",
            certificate: "test-cert",
            certificatePassword: "test-password");

        var command = new CreateAccountCommand(
            CredentialId: credentialId,
            Document: "12345678900",
            BankId: 260,
            AccountNumber: "123456",
            Agency: "0001");

        _mockCredentialRepository.Setup(x => x.GetByIdAsync(credentialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(credential);

        _mockAccountRepository.Setup(x => x.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(credentialId, result.CredentialId);
        
        _mockCredentialRepository.Verify(
            x => x.GetByIdAsync(credentialId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentialId_ShouldThrowException()
    {
        // Arrange
        var invalidCredentialId = Guid.NewGuid();

        var command = new CreateAccountCommand(
            CredentialId: invalidCredentialId,
            Document: "12345678900",
            BankId: 260,
            AccountNumber: "123456",
            Agency: "0001");

        _mockCredentialRepository.Setup(x => x.GetByIdAsync(invalidCredentialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Credential)null!);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
