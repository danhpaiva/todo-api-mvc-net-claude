---
name: check-security
description: Analisa o código modificado no branch atual em busca de vulnerabilidades de segurança. Use antes de abrir um PR ou commitar código sensível.
argument-hint: [arquivo ou contexto específico]
---

Você irá analisar as mudanças do branch atual em busca de vulnerabilidades de segurança.

## Passos

1. Execute `git diff main...HEAD --stat` para listar os arquivos alterados
2. Execute `git diff main...HEAD` para analisar todas as mudanças
3. Se $ARGUMENTS for fornecido, foque no arquivo ou contexto indicado
4. Aplique os critérios abaixo
5. Apresente um relatório estruturado

## Critérios de segurança

### Segredos e credenciais
- Strings hardcoded que se assemelham a senhas, tokens ou chaves JWT nos arquivos `.cs`
- Chaves JWT em `appsettings.json` rastreadas pelo git (devem estar em variáveis de ambiente ou secrets)
- Credenciais fixas em controllers (ex: comparações diretas de `username`/`password` no código)

### Autorização e autenticação
- Endpoints novos sem `[Authorize]` — verificar se a ausência é intencional
- Controllers novos sem política de autorização
- Endpoints de escrita (POST, PUT, DELETE) acessíveis sem autenticação sem justificativa

### Injeção e manipulação de entrada
- Concatenação de strings para montar queries SQL sem uso do EF Core
- Desserialização sem validação (`object` dinâmico sem tipo)
- Falta de `[FromBody]`, `[FromQuery]` ou `[FromRoute]` em parâmetros de controllers
- Ausência de anotações de validação (`[Required]`, `[MaxLength]`, etc.) em DTOs de entrada

### Dados sensíveis em logs
- Campos como `password`, `token`, `secret` sendo logados

### Configuração e exposição
- Swagger habilitado fora de `Development` (verificar `Program.cs`)
- CORS com `AllowAnyOrigin()` sem restrição
- `app.UseDeveloperExceptionPage()` fora de ambiente de desenvolvimento

### Dependências
- Pacotes NuGet adicionados ou alterados — liste os novos e sinalize versões desatualizadas

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

Se o resultado for **REPROVADO**, descreva como reproduzir/explorar cada vulnerabilidade crítica.
