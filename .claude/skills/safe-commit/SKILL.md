---
name: safe-commit
description: Executa verificação de arquitetura, revisão de código, segurança e testes antes de gerar o commit e fazer push. Use no lugar do describe-commit quando quiser garantir qualidade antes de subir o código.
argument-hint: "[contexto opcional]"
---

Você irá executar um pipeline de qualidade antes de commitar e fazer push.

Execute cada etapa na ordem. Se uma etapa crítica falhar, interrompa e informe o desenvolvedor.

---

## Etapa 1 — Verificação de Arquitetura

Leia e siga todas as instruções de:
`.claude/skills/check-architecture/SKILL.md`

Se houver **violações críticas**, interrompa e aguarde confirmação antes de continuar.

---

## Etapa 2 — Revisão de Código

Leia e siga todas as instruções de:
`.claude/skills/review-code/SKILL.md`

Se houver **problemas críticos**, informe e aguarde confirmação.

---

## Etapa 3 — Verificação de Segurança

Leia e siga todas as instruções de:
`.claude/skills/check-security/SKILL.md`

Se houver **vulnerabilidades críticas**, interrompa e aguarde confirmação.

---

## Etapa 4 — Execução dos Testes

```bash
dotnet test TodoApi/TodoApi.Tests/TodoApi.Tests.csproj --logger "console;verbosity=normal"
```

Se algum teste **falhar**, interrompa e informe o nome do teste e a mensagem de erro.

Se não houver testes implementados, registre como aviso e continue.

---

## Etapa 5 — Atualização do README

Leia e siga todas as instruções de:
`.claude/skills/update-readme/SKILL.md`

Se o README foi atualizado, adicione ao stage:
```bash
git add README.md
```

---

## Etapa 6 — Commit e Push

Leia e siga todas as instruções de:
`.claude/skills/describe-commit/SKILL.md`

Se $ARGUMENTS for fornecido, use como contexto adicional para a mensagem de commit.
Pergunte ao desenvolvedor se deseja revisar o commit antes de finalizar.

---

## Resumo final

Apresente:
- Resultado de cada etapa (OK / Avisos / Bloqueado)
- Hash e branch do commit realizado
