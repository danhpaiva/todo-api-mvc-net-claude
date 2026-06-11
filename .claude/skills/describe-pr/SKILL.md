---
name: describe-pr
description: Gera uma descrição completa e padronizada para pull requests. Use quando o usuário pedir para descrever, documentar ou criar a descrição de um PR.
argument-hint: [branch ou contexto opcional]
---

Você irá gerar uma descrição de pull request para o branch atual.

## Passos

1. Execute `git log main..HEAD --oneline` para listar os commits do branch
2. Execute `git diff main...HEAD --stat` para ver os arquivos alterados
3. Execute `git diff main...HEAD` para analisar as mudanças em detalhe
4. Se $ARGUMENTS for fornecido, use como contexto adicional

## Formato da descrição

Gere a descrição em português no seguinte formato:

---

## O que foi feito

<!-- Resumo objetivo em 2-3 frases do que essa PR entrega -->

## Motivação

<!-- Por que essa mudança era necessária? Qual problema resolve? -->

## Mudanças

<!-- Lista com as principais alterações -->
- 

## Como testar

<!-- Passo a passo para validar as mudanças -->
1. 

## Checklist

- [ ] Código revisado pelo autor
- [ ] Testes cobrindo as mudanças
- [ ] Sem quebra de funcionalidades existentes
- [ ] Variáveis de ambiente/configurações documentadas (se aplicável)

---

## Regras

- Seja objetivo e claro, sem enrolação
- Foque no **por que** e não só no **o que**
- Se houver breaking changes, destaque em negrito
- Se os commits forem autoexplicativos, use-os como base para as mudanças
- Não invente contexto — baseie-se apenas no diff e nos commits