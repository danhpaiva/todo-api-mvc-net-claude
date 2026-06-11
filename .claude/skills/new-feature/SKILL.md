---
name: new-feature
description: Cria o esqueleto completo de uma nova feature seguindo a arquitetura do projeto: entidade, interface do serviço, implementação do serviço, controller e registro de DI. Use quando o usuário pedir para criar uma nova funcionalidade do zero.
argument-hint: <NomeDaFeature>
---

Você irá criar todos os arquivos necessários para uma nova feature completa, orquestrando as demais skills do projeto.

## Entrada

O nome da feature é: **$ARGUMENTS**

Se $ARGUMENTS não for fornecido, pergunte o nome antes de prosseguir.

Use o nome fornecido para derivar:
- **NomeDaEntidade**: PascalCase singular (ex: `Produto`)
- **NomeDoServico**: `{NomeDaEntidade}Servico`
- **NomeDoController**: `{NomeDaEntidade}Controller`
- **nomeCamelCase**: camelCase do nome (ex: `produto`)

---

## Passo 1 — Criar a entidade

Execute todos os passos da skill **new-entity** para `{NomeDaEntidade}`.

Isso criará: `TemplateAPI.Entidades/Models/{NomeDaEntidade}.cs`

---

## Passo 2 — Criar a interface do serviço

Crie `TemplateAPI.Servico/Interface/I{NomeDoServico}.cs` seguindo o padrão de `IUsuarioServico.cs`:

```csharp
using TemplateAPI.Entidades.Models;

namespace TemplateAPI.Servico.Interface
{
    public interface I{NomeDoServico}
    {
    }
}
```

> Deixe a interface vazia por ora — os métodos serão adicionados via **new-endpoint** no passo 4.

---

## Passo 3 — Criar a implementação do serviço

Crie `TemplateAPI.Servico/Implementacao/{NomeDoServico}.cs` seguindo o padrão de `UsuarioServico.cs`:

```csharp
using TemplateAPI.Entidades.Models;
using TemplateAPI.Servico.Interface;

namespace TemplateAPI.Servico.Implementacao
{
    public class {NomeDoServico} : I{NomeDoServico}
    {
        public {NomeDoServico}()
        {
        }
    }
}
```

> Deixe a implementação vazia por ora — os métodos serão adicionados via **new-endpoint** no passo 4.

---

## Passo 4 — Criar o shell do controller

Crie `TemplateAPI.API/Controllers/{NomeDoController}.cs` seguindo o padrão de `UsuarioController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using TemplateAPI.API.Controllers.Main;
using TemplateAPI.Servico.Interface;

namespace TemplateAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class {NomeDoController} : MainController
    {
        private readonly I{NomeDoServico} _{nomeCamelCase}Servico;

        public {NomeDoController}(I{NomeDoServico} {nomeCamelCase}Servico, ILogger<{NomeDoController}> logger)
        {
            _{nomeCamelCase}Servico = {nomeCamelCase}Servico;
            _logger = logger;
        }
    }
}
```

---

## Passo 5 — Registrar o serviço no DI

Leia o método `AdicionandoServicos` em `TemplateAPI.API/Program.cs` e adicione:

```csharp
builder.Services.AddScoped<I{NomeDoServico}, {NomeDoServico}>();
```

---

## Passo 6 — Adicionar o endpoint GET inicial

Execute todos os passos da skill **new-endpoint** com os argumentos:

> `{NomeDoController} GET Obter{NomeDaEntidade}`

Isso adicionará o método `GetAsync` ao controller e o método correspondente na interface e implementação do serviço.

---

## Passo 7 — Gerar os testes unitários

Execute todos os passos da skill **generate-tests** para `{NomeDaFeature}`.

Isso criará os testes unitários do serviço e do controller em `TemplateAPI.Teste/`.

---

## Regras

- Mantenha `PascalCase` para classes, métodos e propriedades
- Campos privados com prefixo `_camelCase`
- Todo método de serviço deve ser `async` e aceitar `CancellationToken?`
- Controller não deve ter lógica de negócio
- Sempre registre o serviço no DI em `Program.cs`

## Ao concluir

Liste todos os arquivos criados/modificados e lembre o desenvolvedor de:
1. Adicionar as propriedades reais na entidade
2. Implementar a lógica de negócio nos métodos `TODO` do serviço
3. Ajustar os testes gerados conforme a implementação real do serviço
