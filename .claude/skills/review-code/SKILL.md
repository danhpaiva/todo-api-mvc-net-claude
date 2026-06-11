---
name: review-code
description: Revisa o código modificado no branch atual com foco nos padrões do TodoApi. Use quando o usuário pedir para revisar o código antes de abrir um PR.
argument-hint: [arquivo ou contexto específico]
---

Você irá revisar o código modificado no branch atual seguindo os padrões deste projeto.

## Passos

1. Execute `git diff main...HEAD --stat` para listar os arquivos alterados
2. Execute `git diff main...HEAD` para analisar todas as mudanças
3. Se $ARGUMENTS for fornecido, foque no arquivo ou contexto indicado
4. Aplique os critérios abaixo
5. Apresente um relatório estruturado

## Critérios de revisão

### Nomenclatura
- Classes, métodos, propriedades: `PascalCase`
- Variáveis e parâmetros: `camelCase`
- Campos privados: `_camelCase`
- Interfaces: prefixo `I` (ex: `IMemoryCache`)
- DTOs: sufixo `DTO` (ex: `TodoItemDTO`)

### Controllers
- Não devem conter lógica de negócio além de orquestração HTTP
- Devem injetar dependências via construtor — sem `new` direto
- Cada action deve ter `[Http{Verbo}]` e `[Route]` (ou route no atributo do controller)
- Retornar `ActionResult<T>` ou `IActionResult`
- Ações de escrita (POST, PUT, DELETE) devem invalidar o cache correspondente

### Cache (`IMemoryCache`)
- Sempre usar `TryGetValue` antes de buscar no banco
- Sempre definir `MemoryCacheEntryOptions` com `AbsoluteExpiration` e `SlidingExpiration`
- Toda escrita deve remover as entradas afetadas (`_cache.Remove(...)`)
- Chaves de cache devem ser constantes ou métodos estáticos centralizados no controller

### Models e DTOs
- Entidades EF (`TodoItem`, etc.) não devem ser expostas diretamente nas responses — usar DTOs
- Propriedades de modelos devem usar tipos adequados e nullable quando opcional

### Boas práticas .NET
- Evitar `async void`
- Sem segredos ou connection strings hardcoded
- Sem `using` não utilizados
- Sem código comentado sem justificativa

## Formato do relatório

```
## Revisão de Código

### Problemas críticos
- [arquivo:linha] descrição

### Melhorias sugeridas
- [arquivo:linha] descrição

### Aprovado
- Lista do que está correto e seguindo os padrões
```
