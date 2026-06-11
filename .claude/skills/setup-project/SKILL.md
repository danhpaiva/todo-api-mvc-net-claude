---
name: setup-project
description: Guia o desenvolvedor nos passos iniciais ao clonar o template para um novo projeto. Use quando o usuário quiser iniciar um novo projeto a partir deste template.
argument-hint: [nome do novo projeto]
---

Você irá guiar o desenvolvedor na configuração inicial do projeto a partir deste template.

## Passos

1. Pergunte o nome do novo projeto se $ARGUMENTS não for fornecido
2. Execute `git remote -v` para mostrar a origem atual
3. Apresente o checklist abaixo e execute cada etapa confirmando o sucesso

## Checklist de setup

### 1. Repositório
```bash
# Remover origin do template
git remote remove origin

# Adicionar o novo repositório
git remote add origin {URL_DO_NOVO_REPOSITORIO}

# Push inicial para a branch dev
git push -u origin dev
```

### 2. Renomear o projeto
Renomeie todos os arquivos e referências de `TemplateAPI` para o nome do novo projeto:
- Arquivos `.csproj`
- Namespaces nos arquivos `.cs`
- Nome da solution `.slnx`
- Referências em `Program.cs`
- Referências em `appsettings.json`

Use busca e substituição global: `TemplateAPI` → `{NomeDoProjeto}`

### 3. Configurar variáveis de ambiente
Edite `appsettings.json` e `appsettings.Development.json` com os valores reais:

| Variável | Descrição |
|----------|-----------|
| `RedisCacheSettings.ConnectionString` | String de conexão do Redis |
| `RedisCacheSettings.ReaderEndPoint` | Endpoint de leitura do Redis |
| `SmtpSettings.*` | Configurações de e-mail via AWS SES |
| `CorsSettings.Origin` | Origens permitidas pelo CORS |

### 4. Cognito (autenticação)
Configure as variáveis do Amazon Cognito no `appsettings.json`. Se não for usar autenticação JWT, comente o bloco `CognitoInstaller` em `Program.cs`.

### 5. Redis (cache)
Se não for usar cache, altere `RedisCacheSettings.Enabled` para `false` em `appsettings.json`.

### 6. Verificar a aplicação
```bash
dotnet build
dotnet test
dotnet run --project TemplateAPI.API
```
Acesse o Swagger em `https://localhost:{porta}/swagger` para confirmar que está funcionando.

## Regras

- Execute cada etapa na ordem e confirme o sucesso antes de prosseguir
- Alerte se encontrar arquivos `.env` ou segredos hardcoded
- Informe ao final quais configurações ainda precisam ser preenchidas
