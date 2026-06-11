---
name: check-security
description: Analisa o código modificado no branch atual em busca de vulnerabilidades de segurança comuns: segredos expostos, ausência de autorização, injeção, dados sensíveis em logs e inputs não validados. Use antes de abrir um PR ou commitar código sensível.
argument-hint: [arquivo ou contexto específico]
---

Você irá analisar as mudanças do branch atual em busca de vulnerabilidades de segurança, considerando os padrões deste projeto.

## Passos

1. Execute `git diff main...HEAD --stat` para listar os arquivos alterados
2. Execute `git diff main...HEAD` para analisar todas as mudanças
3. Se $ARGUMENTS for fornecido, foque no arquivo ou contexto indicado
4. Aplique os critérios de segurança abaixo
5. Apresente um relatório estruturado com os problemas encontrados

## Critérios de segurança

### Segredos e credenciais
- Strings hardcoded que se assemelham a chaves, tokens, senhas, connection strings ou ARNs da AWS nos arquivos `.cs` e `.json`
- Chaves em arquivos `appsettings.json` ou `appsettings.*.json` rastreados pelo git (devem estar apenas em ambientes locais ou no secrets manager)
- Qualquer atributo do tipo `password`, `secret`, `key`, `token`, `connectionstring` com valor literal não vazio

### Autorização e autenticação
- Endpoints novos ou alterados sem `[Authorize]` ou com `[AllowAnonymous]` explícito — verifique se a ausência é intencional
- Controllers novos sem política de autorização definida (`[Authorize("Token")]` ou similar)
- Ausência do atributo `[MiddlewareLogs()]` em controllers que manipulam dados de usuário

### Injeção e manipulação de entrada
- Concatenação de strings para montar queries, comandos shell, URLs ou HTML sem sanitização
- Uso de `string.Format` ou interpolação de strings diretamente em chamadas HTTP (Flurl), SQL ou comandos de sistema
- Desserialização de tipos sem validação (`JsonConvert.DeserializeObject` com tipos dinâmicos ou `object`)
- Falta de `[FromBody]`, `[FromQuery]` ou validações de modelo (`[Required]`, `[MaxLength]`, etc.) em parâmetros de entrada de controllers

### Dados sensíveis em logs
- Campos como `senha`, `password`, `cpf`, `token`, `secret`, `creditCard` sendo logados via `_logger`, `Serilog` ou `Console.Write`
- Objetos completos de usuário ou request body sendo serializados em logs sem mascaramento

### Configuração e exposição
- Swagger habilitado fora do ambiente `Local` / `Development` (verificar `Program.cs`)
- CORS com `AllowAnyOrigin()` sem restrição de origem
- Headers de segurança ausentes (`X-Content-Type-Options`, `X-Frame-Options`, `Strict-Transport-Security`) se houver middleware de headers customizado
- `app.UseDeveloperExceptionPage()` fora de ambiente de desenvolvimento

### Dependências
- Pacotes NuGet adicionados ou alterados nos `.csproj` — liste os novos e sinalize se algum for desconhecido ou com versão muito antiga (> 2 anos sem atualização major)

## Formato do relatório

```
## Verificação de Segurança

### Vulnerabilidades críticas
- [arquivo:linha] descrição do problema e como corrigir

### Avisos (risco baixo ou intencional)
- [arquivo:linha] descrição — confirmar se é intencional

### Aprovado
- Lista do que foi verificado e está correto

### Resultado
APROVADO / REPROVADO / APROVADO COM AVISOS — resumo em uma linha
```

Se o resultado for **REPROVADO**, descreva no mínimo como reproduzir/explorar cada vulnerabilidade crítica encontrada, para que o desenvolvedor entenda o impacto real.
