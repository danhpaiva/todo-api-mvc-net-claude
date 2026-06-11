---
name: update-readme
description: Atualiza o README.md com base nas mudanças recentes do projeto. Use quando o usuário pedir para atualizar a documentação, o README ou registrar novos endpoints, dependências ou variáveis de ambiente.
argument-hint: [seção a atualizar ou contexto opcional]
---

Você irá atualizar o `README.md` com base no estado atual do projeto.

## Passos

1. Leia o `README.md` atual
2. Execute `git diff main...HEAD --stat` para ver o que mudou no branch
3. Execute `git diff main...HEAD` para analisar as mudanças em detalhe
4. Identifique o que precisa ser atualizado:
   - Novos endpoints em `TemplateAPI.API/Controllers/`
   - Novas dependências nos arquivos `.csproj`
   - Novas variáveis/seções em `appsettings.json`
   - Mudanças na estrutura de pastas
   - Alterações em `Program.cs` (novos middlewares, serviços registrados)
5. Se $ARGUMENTS for fornecido, foque na seção ou contexto indicado
6. Aplique as atualizações no `README.md` preservando o estilo e tom já existentes

## Regras

- Não remova seções existentes sem justificativa clara
- Mantenha o português e o tom técnico do documento original
- Para novos endpoints, documente: rota, método HTTP e descrição resumida
- Para novas variáveis de ambiente, adicione na seção de dependências ou crie uma seção de configuração se não existir
- Não adicione informações que não estejam evidentes no código ou nas mudanças