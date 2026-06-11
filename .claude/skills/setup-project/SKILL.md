---
name: setup-project
description: Guia o desenvolvedor nos passos iniciais ao clonar o TodoApi para um novo projeto. Use quando o usuário quiser iniciar um novo projeto a partir deste template.
argument-hint: [nome do novo projeto]
---

Você irá guiar o desenvolvedor na configuração inicial do projeto.

## Passos

1. Pergunte o nome do novo projeto se $ARGUMENTS não for fornecido
2. Execute `git remote -v` para mostrar a origem atual
3. Apresente o checklist abaixo e execute cada etapa confirmando o sucesso

## Checklist de setup

### 1. Repositório
```bash
git remote remove origin
git remote add origin {URL_DO_NOVO_REPOSITORIO}
git push -u origin main
```

### 2. Renomear o projeto
Renomeie todas as referências de `TodoApi` para o nome do novo projeto:
- Arquivos `.csproj` e `.sln`
- Namespaces nos arquivos `.cs`
- Referências em `Program.cs`

Use busca e substituição global: `TodoApi` → `{NomeDoProjeto}`

### 3. Configurar JWT
Edite `appsettings.json` com os valores reais:

| Variável | Descrição |
|----------|-----------|
| `Jwt:Key` | Chave secreta para assinar os tokens (mínimo 32 caracteres) |
| `Jwt:Issuer` | Identificador do emissor do token |
| `Jwt:Audience` | Identificador do público-alvo do token |

> ⚠️ Nunca commite a `Jwt:Key` com valor real — use variáveis de ambiente ou `dotnet user-secrets` em desenvolvimento.

### 4. Banco de dados
O projeto usa `InMemoryDatabase` por padrão. Para trocar para um banco real:
1. Adicione o pacote do provider desejado (ex: `Microsoft.EntityFrameworkCore.SqlServer`)
2. Substitua `opt.UseInMemoryDatabase(...)` em `Program.cs` pela string de conexão
3. Execute `dotnet ef migrations add InitialCreate` e `dotnet ef database update`

### 5. Verificar a aplicação
```bash
dotnet build
dotnet test
dotnet run --project TodoApi/TodoApi
```
Acesse o Swagger em `https://localhost:{porta}/swagger`.

## Regras

- Execute cada etapa na ordem e confirme o sucesso antes de prosseguir
- Alerte se encontrar arquivos com segredos hardcoded
- Informe ao final quais configurações ainda precisam ser preenchidas
