---
name: generate-tests
description: Gera testes unitários completos para um controller e seu serviço correspondente, cobrindo todos os endpoints. Use quando o usuário pedir para criar ou gerar testes de uma feature.
argument-hint: <NomeDaFeature>
---

Você irá gerar testes unitários para o controller e o serviço de uma feature.

## Entrada

O nome da feature é: **$ARGUMENTS**

Se $ARGUMENTS não for fornecido, pergunte o nome antes de prosseguir.

Derive os nomes:
- **NomeDaEntidade**: PascalCase singular (ex: `Produto`)
- **NomeDoServico**: `{NomeDaEntidade}Servico`
- **NomeDoController**: `{NomeDaEntidade}Controller`
- **nomeCamelCase**: camelCase do nome (ex: `produto`)

## Passos

### 1. Leitura dos arquivos-fonte

Leia os seguintes arquivos para entender o que testar:

- `TemplateAPI.API/Controllers/{NomeDoController}.cs` — endpoints existentes
- `TemplateAPI.Servico/Interface/I{NomeDoServico}.cs` — contrato do serviço
- `TemplateAPI.Servico/Implementacao/{NomeDoServico}.cs` — implementação e dependências injetadas

### 2. Verificar e configurar o projeto de testes

Leia `TemplateAPI.Teste/TemplateAPI.Teste.csproj`. Se o pacote `Moq` **não** estiver presente, adicione-o:

```xml
<PackageReference Include="Moq" Version="4.20.72" />
```

Verifique também se a referência ao projeto `TemplateAPI.Servico` e `TemplateAPI.API` estão presentes no `.csproj`. Se não estiverem, adicione:

```xml
<ItemGroup>
  <ProjectReference Include="..\TemplateAPI.Servico\TemplateAPI.Servico.csproj" />
  <ProjectReference Include="..\TemplateAPI.API\TemplateAPI.API.csproj" />
  <ProjectReference Include="..\TemplateAPI.Entidades\TemplateAPI.Entidades.csproj" />
</ItemGroup>
```

### 3. Gerar testes do serviço — `TemplateAPI.Teste/Servicos/{NomeDoServico}Tests.cs`

- Identifique todas as dependências do construtor do serviço (ex: `IDatabaseApi`, `IEmail`, etc.)
- Crie um mock para cada dependência usando `Mock<T>`
- Para cada método público da interface, gere ao menos dois cenários:
  - **Caminho feliz**: entrada válida, retorno esperado
  - **Caminho de erro**: entrada inválida ou exceção esperada

Padrão do arquivo:

```csharp
using Moq;
using TemplateAPI.Entidades.Models;
using TemplateAPI.Servico.Implementacao;
// adicionar usings das interfaces mockadas conforme as dependências reais

namespace TemplateAPI.Teste.Servicos
{
    public class {NomeDoServico}Tests
    {
        // Declare um Mock<T> para cada dependência do construtor
        // private readonly Mock<IDependencia> _dependenciaMock;
        private readonly {NomeDoServico} _servico;

        public {NomeDoServico}Tests()
        {
            // _dependenciaMock = new Mock<IDependencia>();
            _servico = new {NomeDoServico}(/* passar _dependenciaMock.Object conforme necessário */);
        }

        // Para cada método da interface, gere os cenários abaixo:

        [Fact]
        public async Task {NomeDoMetodo}_Com{CondicaoValida}_Deve{RetornoEsperado}()
        {
            // Arrange
            var id = Guid.NewGuid();
            // configurar mocks conforme a implementação real

            // Act
            var resultado = await _servico.{NomeDoMetodo}(id);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task {NomeDoMetodo}_Com{CondicaoInvalida}_Deve{ExcecaoOuComportamentoEsperado}()
        {
            // Arrange
            // ...

            // Act & Assert
            await Assert.ThrowsAsync<{TipoDeExcecao}>(
                () => _servico.{NomeDoMetodo}(/* entrada inválida */));
        }
    }
}
```

### 4. Gerar testes do controller — `TemplateAPI.Teste/Controllers/{NomeDoController}Tests.cs`

- Mock da interface do serviço (`Mock<I{NomeDoServico}>`)
- Para cada action do controller, gere ao menos três cenários:
  - **Retorno 200 OK**: serviço retorna dado válido
  - **Retorno 204 No Content**: serviço retorna `null`
  - **Retorno 400 Bad Request**: serviço lança exceção

Padrão do arquivo:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TemplateAPI.API.Controllers;
using TemplateAPI.Entidades.Models;
using TemplateAPI.Servico.Interface;

namespace TemplateAPI.Teste.Controllers
{
    public class {NomeDoController}Tests
    {
        private readonly Mock<I{NomeDoServico}> _{nomeCamelCase}ServicoMock;
        private readonly Mock<ILogger<{NomeDoController}>> _loggerMock;
        private readonly {NomeDoController} _controller;

        public {NomeDoController}Tests()
        {
            _{nomeCamelCase}ServicoMock = new Mock<I{NomeDoServico}>();
            _loggerMock = new Mock<ILogger<{NomeDoController}>>();
            _controller = new {NomeDoController}(_{nomeCamelCase}ServicoMock.Object, _loggerMock.Object);
        }

        // Repita o bloco abaixo para cada action do controller:

        [Fact]
        public async Task {NomeDoEndpoint}Async_QuandoServicoRetornaDado_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade = new {NomeDaEntidade} { Id = id };
            _{nomeCamelCase}ServicoMock
                .Setup(s => s.{NomeDoMetodoDoServico}(id, It.IsAny<CancellationToken?>()))
                .ReturnsAsync(entidade);

            // Act
            var resultado = await _controller.{NomeDoEndpoint}Async(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var retorno = Assert.IsType<{NomeDaEntidade}>(okResult.Value);
            Assert.Equal(id, retorno.Id);
        }

        [Fact]
        public async Task {NomeDoEndpoint}Async_QuandoServicoRetornaNull_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            _{nomeCamelCase}ServicoMock
                .Setup(s => s.{NomeDoMetodoDoServico}(id, It.IsAny<CancellationToken?>()))
                .ReturnsAsync((({NomeDaEntidade}?)null));

            // Act
            var resultado = await _controller.{NomeDoEndpoint}Async(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public async Task {NomeDoEndpoint}Async_QuandoServicoLancaExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            _{nomeCamelCase}ServicoMock
                .Setup(s => s.{NomeDoMetodoDoServico}(id, It.IsAny<CancellationToken?>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var resultado = await _controller.{NomeDoEndpoint}Async(id, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }
    }
}
```

## Regras

- Nomeie métodos de teste no padrão: `{Metodo}_{Condicao}_{ResultadoEsperado}`
- Use `[Fact]` para casos determinísticos e `[Theory] + [InlineData]` para variações de entrada
- Um teste por cenário — nunca valide duas coisas diferentes no mesmo `[Fact]`
- Não instancie objetos reais de infraestrutura (banco, HTTP) — use sempre mocks
- Crie a pasta `Servicos/` e `Controllers/` dentro de `TemplateAPI.Teste/` se não existirem

## Ao concluir

Liste os arquivos criados/modificados e informe quantos cenários de teste foram gerados por classe.
