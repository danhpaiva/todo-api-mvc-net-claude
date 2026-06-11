---
name: generate-tests
description: Gera testes unitários para um controller do TodoApi, cobrindo todos os endpoints. Use quando o usuário pedir para criar ou gerar testes de uma feature.
argument-hint: <NomeDoController>
---

Você irá gerar testes unitários para um controller.

## Entrada

O nome do controller é: **$ARGUMENTS**

Se $ARGUMENTS não for fornecido, pergunte o nome antes de prosseguir.

Derive:
- **NomeDoController**: `{Nome}sController` (ex: `TodoItemsController`)
- **NomeDaEntidade**: singular sem sufixo (ex: `TodoItem`)
- **NomeDaEntidadeDTO**: `{NomeDaEntidade}DTO`

## Passos

### 1. Ler o controller-fonte

Leia `TodoApi/Controllers/{NomeDoController}.cs` para entender:
- Quais dependências são injetadas (sempre haverá `AppDbContext` e `IMemoryCache`)
- Quais endpoints existem e quais status HTTP cada um retorna

### 2. Verificar o projeto de testes

Verifique se `TodoApi.Tests/Controllers/` existe. Se não existir, crie a pasta.

Confirme que `TodoApi.Tests/TodoApi.Tests.csproj` tem referência ao projeto principal:
```xml
<ProjectReference Include="..\TodoApi\TodoApi.csproj" />
```

### 3. Gerar o arquivo de testes

Crie `TodoApi.Tests/Controllers/{NomeDoController}Tests.cs` seguindo o padrão abaixo.

**Padrão de setup** (baseado em `TodoItemsControllerTests.cs`):

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TodoApi.Context;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests.Controllers;

public class {NomeDoController}Tests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IMemoryCache GetMemoryCache() =>
        new MemoryCache(new MemoryCacheOptions());

    // Para cada endpoint do controller, gere os cenários abaixo:

    [Fact]
    public async Task {NomeDoEndpoint}_Quando{CondicaoValida}_Deve{RetornoEsperado}()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var cache = GetMemoryCache();
        context.{DbSet}.Add(new {NomeDaEntidade} { Id = 1, /* propriedades */ });
        await context.SaveChangesAsync();
        var controller = new {NomeDoController}(context, cache);

        // Act
        var result = await controller.{NomeDoEndpoint}(/* params */);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<{NomeDaEntidadeDTO}>(okResult.Value);
        Assert.Equal(1, dto.Id);
    }
}
```

**Cenários obrigatórios por tipo de endpoint:**

| Endpoint | Cenários mínimos |
|----------|-----------------|
| GET (lista) | retorna lista com itens; retorna lista vazia |
| GET (por id) | id válido → retorna DTO; id inválido → 404 |
| POST | cria item → 201 CreatedAtAction com DTO |
| PUT | id coincide + item existe → 204; ids diferentes → 400; item não existe → 404 |
| DELETE | item existe → 204 + removido do banco; item não existe → 404 |

## Regras

- Um `[Fact]` por cenário — nunca valide duas coisas diferentes no mesmo teste
- Sempre crie um novo `DbContext` e `MemoryCache` por teste (evita estado compartilhado)
- Use `Guid.NewGuid().ToString()` como nome do banco InMemory para isolamento
- Nomeie no padrão: `{Endpoint}_{Condicao}_{ResultadoEsperado}`
- Use `[Theory] + [InlineData]` para variações de entrada (ex: ids inválidos)
- Não use mocks para `AppDbContext` — use o InMemory provider do EF

## Ao concluir

Liste os arquivos criados/modificados e informe quantos cenários foram gerados por controller.
