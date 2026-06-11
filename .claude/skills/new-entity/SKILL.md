---
name: new-entity
description: Cria uma nova entidade e seu DTO correspondente na pasta Models. Use quando o usuário pedir para criar uma entidade, modelo ou classe de domínio.
argument-hint: <NomeDaEntidade>
---

Você irá criar uma nova entidade e seu DTO em `TodoApi/Models/`.

## Entrada

O nome da entidade é: **$ARGUMENTS**

Se $ARGUMENTS não for fornecido, pergunte o nome antes de prosseguir.

## Arquivos a criar

### 1. Entidade — `TodoApi/Models/{NomeDaEntidade}.cs`

Siga o padrão de `TodoItem.cs`:

```csharp
namespace TodoApi.Models
{
    public class {NomeDaEntidade}
    {
        public long Id { get; set; }

        // Adicione as propriedades específicas aqui
    }
}
```

### 2. DTO — `TodoApi/Models/{NomeDaEntidade}DTO.cs`

Siga o padrão de `TodoItemDTO.cs`:

```csharp
namespace TodoApi.Models
{
    public class {NomeDaEntidade}DTO
    {
        public long Id { get; set; }

        // Inclua apenas as propriedades que devem ser expostas ao cliente
    }
}
```

### 3. Registrar no DbContext — `TodoApi/Context/AppDbContext.cs`

Adicione o `DbSet` correspondente:

```csharp
public DbSet<{NomeDaEntidade}> {NomeDaEntidade}s { get; set; } = null!;
```

## Regras

- Use `PascalCase` para nomes de classe e propriedades
- O Id deve ser `long` para consistência com o restante do projeto
- Propriedades opcionais devem ser `nullable` (ex: `string?`)
- Nunca exponha a entidade diretamente nas responses — sempre use o DTO
- Sem lógica de negócio nas entidades — apenas propriedades

## Após criar

Informe os arquivos criados/modificados e sugira o próximo passo:
- Criar o controller com `/new-feature {NomeDaEntidade}` para o fluxo completo
- Ou adicionar apenas um endpoint com `/new-endpoint`
