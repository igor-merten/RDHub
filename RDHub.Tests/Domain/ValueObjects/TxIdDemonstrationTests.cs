using RDHub.Domain.ValueObjects;
using Xunit;

namespace RDHub.Tests.Domain.ValueObjects;

public class TxIdDemonstrationTests
{
    [Fact]
    public void Demo_TestQuePassaComSucesso()
    {
        // Arrange
        var validValue = "TXN123456789";

        // Act
        var txId = TxId.From(validValue);

        // Assert - Este teste PASSA
        Assert.Equal(validValue, txId.Value);
    }

    [Fact]
    public void Demo_TestQueFalha_IntencionalmentePraVocePoder()
    {
        // Este teste foi criado para mostrar como é o output quando algo falha!
        // Descomente a linha abaixo para ver o teste falhando
        
        // Assert.Equal("valor_esperado", "valor_diferente");
        
        // Por enquanto deixaremos comentado para não quebrar a build
        Assert.True(true);
    }
}
