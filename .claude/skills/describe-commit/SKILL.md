---
name: describe-commit
description: Gera uma mensagem de commit padronizada, executa o commit e faz push. Use quando o usuário pedir para commitar ou enviar as mudanças.
argument-hint: [contexto opcional]
---

Você irá gerar a mensagem de commit, executar o commit e fazer push das mudanças.

## Passos

1. Execute `git diff --cached --stat` para ver os arquivos em stage
2. Execute `git diff --cached` para analisar as mudanças em detalhe
3. Se não houver nada em stage, use `git diff --stat` e `git diff` para ver mudanças não staged
4. Se $ARGUMENTS for fornecido, use como contexto adicional
5. Gere a mensagem seguindo o formato abaixo
6. Execute o commit: `git commit -m "<mensagem gerada>"`
7. Execute o push: `git push`
8. Confirme o sucesso informando o hash do commit e o branch em que foi feito o push

## Formato da mensagem

Use o padrão **Conventional Commits** em português:

```
<tipo>(<escopo opcional>): <descrição curta no imperativo>

<corpo opcional — explica o porquê, não o quê>

<rodapé opcional — breaking changes, issues relacionadas>
```

### Tipos permitidos

| Tipo | Quando usar |
|------|-------------|
| `feat` | Nova funcionalidade |
| `fix` | Correção de bug |
| `refactor` | Refatoração sem mudança de comportamento |
| `chore` | Tarefas de manutenção, dependências, configurações |
| `docs` | Documentação |
| `test` | Adição ou correção de testes |
| `style` | Formatação, espaços, ponto e vírgula |
| `perf` | Melhoria de performance |
| `ci` | Mudanças em CI/CD |

## Exemplos

```
feat(auth): adiciona autenticação via JWT

fix(pdf): corrige geração de PDF quando o template está vazio

refactor(usuario): extrai lógica de validação para classe separada

chore: atualiza dependências do projeto
```

## Regras

- Descrição curta: máximo 72 caracteres, imperativo, sem ponto final
- Corpo: use quando o **porquê** não é óbvio pelo título
- Não mencione nomes de arquivos no título — isso fica no diff
- Se houver breaking change, adicione `BREAKING CHANGE:` no rodapé
- Execute o commit e o push sem pedir confirmação, a menos que haja arquivos sensíveis (.env, segredos) em stage
