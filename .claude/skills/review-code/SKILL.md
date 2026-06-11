---
name: review-code
description: Revisa o código modificado no branch atual com foco nos padrões do projeto. Use quando o usuário pedir para revisar o código antes de abrir um PR.
argument-hint: [arquivo ou contexto específico]
---

Você irá revisar o código modificado no branch atual seguindo os padrões deste projeto.

## Passos

1. Execute `git diff main...HEAD --stat` para listar os arquivos alterados
2. Execute `git diff main...HEAD` para analisar todas as mudanças
3. Se $ARGUMENTS for fornecido, foque no arquivo ou contexto indicado
4. Aplique os critérios de revisão abaixo
5. Apresente um relatório estruturado com os problemas encontrados e sugestões

## Critérios de revisão

### Nomenclatura (CONTRIBUTING.md)
- Classes, métodos, propriedades e constantes: `PascalCase`
- Variáveis, parâmetros: `camelCase`
- Campos privados: `_camelCase`
- Interfaces: prefixo `I` (ex: `IUsuarioServico`)
- Arquivos: `PascalCase.cs`

### Arquitetura em camadas
Verifique se as dependências entre camadas estão corretas:

| Camada | Pode depender de |
|--------|-----------------|
| `TemplateAPI.API` | `Servico`, `Transversais`, `Entidades` |
| `TemplateAPI.Servico` | `Integracoes`, `Entidades`, `Transversais` |
| `TemplateAPI.Integracoes` | `Entidades`, `Transversais` |
| `TemplateAPI.Entidades` | `Transversais` |
| `TemplateAPI.Transversais` | nenhuma camada interna |

Sinalize se alguma camada estiver importando outra que não deveria.

### Controllers
- Não devem conter lógica de negócio
- Devem delegar para a camada `Servico`
- Devem retornar tipos padronizados (`IActionResult` ou `ActionResult<T>`)
- Devem ter atributos de rota e verbo HTTP explícitos

### Boas práticas .NET
- Evitar `async void` (exceto event handlers)
- Usar `CancellationToken` em operações assíncronas longas
- Não capturar exceções genéricas sem re-throw ou log
- Injeção de dependência via construtor (não `new` direto em serviços)
- Sem segredos ou strings de conexão hardcoded

### Qualidade geral
- Métodos com mais de 30 linhas devem ser justificados ou refatorados
- Sem código comentado ou TODO sem contexto
- Sem imports não utilizados

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