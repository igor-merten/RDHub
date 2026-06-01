using RDHub.Domain.ValueObjects;
using RDHub.Domain.Exceptions;
using Xunit;

namespace RDHub.Tests.Domain.ValueObjects;

public class TxIdTests
{
    [Fact]
    public void From_WithValidValue_ShouldCreateTxId()
    {
        // Arrange
        var validValue = "TXN123456789";

        // Act
        var txId = TxId.From(validValue);

        // Assert
        Assert.NotNull(txId);
        Assert.Equal(validValue, txId.Value);
    }

    [Theory]
    [InlineData("")]
    public void From_WithEmptyOrNullValue_ShouldThrowException(string invalidValue)
    {
        // Act & Assert
        Assert.Throws<InvalidTxIdException>(() => TxId.From(invalidValue));
    }

    [Fact]
    public void From_WithValueExceeding35Characters_ShouldThrowException()
    {
        // Arrange
        var invalidValue = "A123456789B123456789C123456789D12345"; // 37 caracteres

        // Act & Assert
        Assert.Throws<InvalidTxIdException>(() => TxId.From(invalidValue));
    }

    [Fact]
    public void From_WithSpecialCharacters_ShouldThrowException()
    {
        // Arrange
        var invalidValue = "TXN@123#456";

        // Act & Assert
        Assert.Throws<InvalidTxIdException>(() => TxId.From(invalidValue));
    }

    [Fact]
    public void Generate_ShouldCreateValidTxId()
    {
        // Act
        var txId = TxId.Generate();

        // Assert
        Assert.NotNull(txId);
        Assert.False(string.IsNullOrEmpty(txId.Value));
        Assert.True(txId.Value.Length <= 35);
        Assert.StartsWith("RDH", txId.Value);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertTxIdToString()
    {
        // Arrange
        var validValue = "TXN987654321";
        var txId = TxId.From(validValue);

        // Act
        string result = txId;

        // Assert
        Assert.Equal(validValue, result);
    }
}
