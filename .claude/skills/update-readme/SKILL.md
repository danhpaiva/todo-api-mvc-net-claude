---
name: update-readme
description: Atualiza o README.md com base nas mudanças recentes do projeto. Use quando o usuário pedir para atualizar a documentação ou registrar novos endpoints e dependências.
argument-hint: [seção a atualizar ou contexto opcional]
---

Você irá atualizar o `README.md` com base no estado atual do projeto.

## Passos

1. Leia o `README.md` atual
2. Execute `git diff main...HEAD --stat` para ver o que mudou
3. Execute `git diff main...HEAD` para analisar as mudanças em detalhe
4. Identifique o que precisa ser atualizado:
   - Novos controllers ou endpoints em `TodoApi/Controllers/`
   - Novos pacotes NuGet em `TodoApi/TodoApi.csproj`
   - Novas configurações em `appsettings.json`
   - Mudanças em `Program.cs` (novos middlewares, serviços registrados)
5. Se $ARGUMENTS for fornecido, foque na seção ou contexto indicado
6. Aplique as atualizações preservando o estilo e tom do documento original

## Regras

- Não remova seções existentes sem justificativa
- Mantenha o português e o tom técnico do documento original
- Para novos endpoints, documente: rota, método HTTP e descrição resumida
- Para novas configurações, adicione na seção correspondente
- Não adicione informações que não estejam evidentes no código
