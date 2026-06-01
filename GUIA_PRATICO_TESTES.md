# 🎯 Guia Prático: Como Ver os Testes Funcionando

## 1. Executar TODOS os testes

```powershell
cd c:\Users\joao\Desktop\residencia\RDHub
dotnet test RDHub.Tests
```

**Output esperado:**
```
Resumo do teste: total: 17; falhou: 0; bem-sucedido: 17; ignorado: 0
```

---

## 2. Executar com SAÍDA DETALHADA

Mostra cada teste passando individualmente:

```powershell
dotnet test RDHub.Tests --logger "console;verbosity=detailed"
```

**Você verá algo como:**
```
✓ Aprovado RDHub.Tests.Domain.ValueObjects.TxIdTests.From_WithValidValue_ShouldCreateTxId [5 ms]
✓ Aprovado RDHub.Tests.Domain.Aggregates.AccountTests.Create_WithValidData_ShouldCreateAccount [7 ms]
✓ Aprovado RDHub.Tests.Application.Commands.CreateAccountHandlerTests.Handle_WithValidCommand_ShouldCreateAccount [9 ms]
```

---

## 3. Executar TESTES ESPECÍFICOS (por filtro)

### Apenas testes de TxId:
```powershell
dotnet test RDHub.Tests --filter "TxIdTests"
```

### Apenas testes de Account:
```powershell
dotnet test RDHub.Tests --filter "AccountTests"
```

### Apenas testes de Handler:
```powershell
dotnet test RDHub.Tests --filter "HandleTests"
```

**Output:**
```
Resumo do teste: total: 6; falhou: 0; bem-sucedido: 6
```

---

## 4. Ver o que Acontece Quando um Teste FALHA

### Descomente este código em `TxIdDemonstrationTests.cs`:

```csharp
[Fact]
public void Demo_TestQueFalha_IntencionalmentePraVocePoder()
{
    Assert.Equal("valor_esperado", "valor_diferente");  // ← DESCOMENTE ISTO
}
```

### Execute:
```powershell
dotnet test RDHub.Tests --filter "Demo_TestQueFalha"
```

### Você verá:
```
✗ Falhado RDHub.Tests.Domain.ValueObjects.TxIdDemonstrationTests.Demo_TestQueFalha_IntencionalmentePraVocePoder

Expected: valor_esperado
Actual:   valor_diferente

Stack Trace:
    at RDHub.Tests.Domain.ValueObjects.TxIdDemonstrationTests.Demo_TestQueFalha_IntencionalmentePraVocePoder() 
    in C:\Users\joao\Desktop\residencia\RDHub\RDHub.Tests\...
```

---

## 5. Ver Tempo de Execução de Cada Teste

```powershell
dotnet test RDHub.Tests -v normal
```

**Mostra:**
```
RDHub.Tests.Domain.ValueObjects.TxIdTests.From_WithValidValue_ShouldCreateTxId [5 ms]
RDHub.Tests.Domain.Aggregates.AccountTests.Create_WithValidData_ShouldCreateAccount [7 ms]
RDHub.Tests.Application.Commands.CreateAccountHandlerTests.Handle_WithValidCredentialId_ShouldVerifyCredentialExists [92 ms]
```

---

## 6. Executar NO VS CODE (Integrado)

### Opção 1: Extensão Test Explorer
1. Instale: "Test Explorer UI" (`hbenl.test-explorer`)
2. Veja os testes na sidebar
3. Clique em ▶ para rodar
4. Clique em 🔴 para ver detalhes do erro

### Opção 2: VS Code Command
1. Pressione `Ctrl+Shift+P`
2. Digite: `C# Test Run`
3. Selecione o teste

---

## 7. Entender a Estrutura dos Testes

### Um teste bem-escrito tem 3 partes: **AAA**

```csharp
[Fact]
public void Create_WithValidData_ShouldCreateAccount()
{
    // ✅ ARRANGE - Prepara os dados
    var credentialId = Guid.NewGuid();
    var document = "12345678900";
    
    // ✅ ACT - Executa a ação
    var account = Account.Create(credentialId, document, 260, "123456", "0001");
    
    // ✅ ASSERT - Valida o resultado
    Assert.NotNull(account);
    Assert.Equal(credentialId, account.CredentialId);
    Assert.Equal(document, account.Document);
}
```

---

## 8. Exemplo Real: Validar que TxId rejeita caracteres especiais

```csharp
[Fact]
public void From_WithSpecialCharacters_ShouldThrowException()
{
    // Arrange
    var invalidValue = "TXN@123#456";  // ← Contém @, #
    
    // Act & Assert
    Assert.Throws<InvalidTxIdException>(() => TxId.From(invalidValue));
    //                                 ↑
    //                        Esperamos que lance exceção!
}
```

**Resultado:** ✓ Aprovado

---

## 9. Verificar Cobertura de Código (Opcional)

```powershell
# Se tiver OpenCover instalado
dotnet test RDHub.Tests /p:CollectCoverage=true /p:CoverageFormat=opencover
```

---

## 📊 Checklist Prático

- [ ] Execute todos os testes: `dotnet test RDHub.Tests`
- [ ] Veja saída detalhada: `dotnet test RDHub.Tests --logger "console;verbosity=detailed"`
- [ ] Filtre por TxId: `dotnet test RDHub.Tests --filter "TxIdTests"`
- [ ] Filtre por Account: `dotnet test RDHub.Tests --filter "AccountTests"`
- [ ] Filtre por Handler: `dotnet test RDHub.Tests --filter "HandleTests"`
- [ ] Descomente demo de falha e veja o erro
- [ ] Comente a demo de falha de novo
- [ ] Veja tempo de execução de cada teste

---

## 🎯 Resumo

| Comando | Uso |
|---|---|
| `dotnet test RDHub.Tests` | Roda todos |
| `dotnet test RDHub.Tests --filter "TxIdTests"` | Roda específico |
| `dotnet test RDHub.Tests -v normal` | Mostra detalhes |
| `dotnet test RDHub.Tests --logger "console;verbosity=detailed"` | Máxima verbosidade |

---

## 💡 O que os testes garantem:

✅ **TxId não aceita valores vazios**
✅ **TxId rejeita > 35 caracteres**
✅ **TxId rejeita caracteres especiais**
✅ **Account não pode ter BankId = 0**
✅ **Account não pode ser desativada 2x**
✅ **CreateAccountHandler verifica credencial**
✅ **CreateAccountHandler salva corretamente**

---

Próxima vez que quiser rodar, é só `dotnet test RDHub.Tests` 🚀
