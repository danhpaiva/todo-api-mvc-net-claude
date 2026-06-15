---
name: check-architecture
description: Verifica violações de arquitetura no projeto TodoApi. Use quando quiser garantir que a estrutura de pastas, responsabilidades e dependências estão corretas.
argument-hint: [pasta ou arquivo específico]
---

Você irá verificar se o projeto respeita a arquitetura definida para o `TodoApi`.

## Passos

1. Se $ARGUMENTS for fornecido, foque na pasta ou arquivo indicado; caso contrário, analise todo o projeto
2. Inspecione os arquivos `.cs` e `Program.cs` buscando violações das regras abaixo
3. Apresente um relatório com violações encontradas e como corrigi-las

## Estrutura esperada do projeto

```
TodoApi/
  Controllers/         — orquestração HTTP, validação de entrada, respostas
  Context/             — AppDbContext e configuração do EF Core
  Models/              — entidades, DTOs e modelos de entrada
  Services/            — lógica de negócio, cache, orquestração de dados
    Interfaces/        — contratos de serviço (I*Service)
  Program.cs           — registro de DI e pipeline de middlewares
TodoApi.Tests/
  Controllers/         — testes unitários dos controllers
```

## Regras a verificar

### Controllers (`TodoApi/Controllers/`)
- Apenas orquestração HTTP: receber request, chamar serviço, retornar response
- Não devem conter lógica de negócio complexa (cálculos, regras de domínio, transformações pesadas)
- Devem injetar interfaces de serviço via construtor (`ITodoService`, `IAuthService`, etc.) — não `AppDbContext` diretamente
- Não devem instanciar dependências com `new` (ex: `new AppDbContext(...)`)
- Devem ter `[Route]` e verbos HTTP explícitos em cada action
- Retornar `ActionResult<T>` ou `IActionResult`

### Services (`TodoApi/Services/`)
- Contém a lógica de negócio, queries ao banco e manipulação de cache
- Cada serviço deve implementar sua interface correspondente em `Services/Interfaces/`
- Interfaces (`I*Service`) devem ficar em `Services/Interfaces/`
- Não devem retornar tipos de infraestrutura (`IQueryable`, `DbSet`) — retornar DTOs ou entidades mapeadas
- Cache (`IMemoryCache`) deve ser gerenciado aqui, não nos controllers

### Context (`TodoApi/Context/`)
- Apenas `DbContext` e sua configuração (`DbSet`, `OnModelCreating`)
- Não deve conter lógica de negócio ou manipulação de cache

### Models (`TodoApi/Models/`)
- Apenas propriedades de dados — sem lógica de negócio
- DTOs (`*DTO`) usados para transferência de dados entre controller e cliente
- Entidades usadas pelo EF Core para persistência

### Program.cs
- Apenas registro de serviços (`builder.Services.*`) e configuração de middlewares (`app.Use*`, `app.Map*`)
- Não deve conter lógica de negócio inline

### Boas práticas gerais
- Sem instanciação direta de dependências com `new` dentro de controllers ou serviços
- Sem segredos ou strings de conexão hardcoded nos arquivos `.cs`
- Cache (`IMemoryCache`) deve ser gerenciado nos serviços e invalidado em toda operação de escrita (POST, PUT, DELETE)
- Testes de controller devem mockar interfaces de serviço (`I*Service`), não dependências de infraestrutura (`AppDbContext`, `IMemoryCache`)

## Formato do relatório

```
## Verificação de Arquitetura

### Violações encontradas
- [arquivo:linha] descrição da violação e como corrigir

### Avisos (não críticos)
- [arquivo:linha] descrição

### Resultado
APROVADO / REPROVADO — resumo em uma linha
```
