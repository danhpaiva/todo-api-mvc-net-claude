---
name: safe-commit
description: Executa verificação de arquitetura, revisão de código, testes unitários e atualização do README antes de gerar o commit e fazer push. Use no lugar do describe-commit quando quiser garantir qualidade antes de subir o código.
argument-hint: "[contexto opcional]"
---

Você irá executar um pipeline completo de qualidade antes de commitar e fazer push.

Execute cada etapa na ordem abaixo. Se uma etapa crítica falhar, interrompa e informe o desenvolvedor antes de prosseguir.

---

## Etapa 1 — Verificação de Arquitetura

Leia e siga todas as instruções do arquivo:
`.claude/skills/check-architecture/SKILL.md`

Se houver **violações críticas**, interrompa aqui e liste os problemas. Não prossiga para as próximas etapas até que o desenvolvedor confirme ou corrija.

---

## Etapa 2 — Revisão de Código

Leia e siga todas as instruções do arquivo:
`.claude/skills/review-code/SKILL.md`

Se houver **problemas críticos**, informe o desenvolvedor e aguarde confirmação para continuar.

---

## Etapa 3 — Verificação de Segurança

Leia e siga todas as instruções do arquivo:
`.claude/skills/check-security/SKILL.md`

Se houver **vulnerabilidades críticas**, interrompa aqui e liste os problemas. Não prossiga para as próximas etapas até que o desenvolvedor confirme ou corrija.

---

## Etapa 4 — Execução dos Testes Unitários

Execute os testes do projeto `TemplateAPI.Teste`:

```bash
dotnet test TemplateAPI.Teste/TemplateAPI.Teste.csproj --logger "console;verbosity=normal"
```

Se algum teste **falhar**, interrompa aqui e informe o desenvolvedor com o nome do teste e a mensagem de erro. Não prossiga para as próximas etapas até que os testes passem ou o desenvolvedor confirme explicitamente que deseja continuar mesmo assim.

Se o projeto de testes **não possuir nenhum teste implementado**, registre como aviso e continue.

---

## Etapa 5 — Atualização do README

Leia e siga todas as instruções do arquivo:
`.claude/skills/update-readme/SKILL.md`

Se o README foi atualizado, adicione o arquivo ao stage:
```bash
git add README.md
```

---

## Etapa 6 — Commit e Push

Leia e siga todas as instruções do arquivo:
`.claude/skills/describe-commit/SKILL.md`

Se necessário divida o commit em múltiplos commits menores, seguindo as melhores práticas de mensagens de commit.
Se $ARGUMENTS for fornecido, use como contexto adicional para a mensagem de commit.
Sempre pergunte ao desenvolvedor se ele deseja revisar o commit antes de finalizar. Se o desenvolvedor solicitar alterações, ajuste a mensagem conforme necessário.

---

## Resumo final

Ao concluir, apresente um resumo com:
- Resultado de cada etapa (OK / Avisos / Bloqueado)
- Hash e branch do commit realizado
