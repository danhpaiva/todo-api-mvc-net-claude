---
name: check-architecture
description: Verifica violações de arquitetura em camadas no projeto. Use quando quiser garantir que as dependências entre camadas estão corretas e que a lógica está no lugar certo.
argument-hint: [camada ou arquivo específico]
---

Você irá verificar se o projeto respeita a arquitetura em camadas definida pelo template.

## Passos

1. Se $ARGUMENTS for fornecido, foque na camada ou arquivo indicado; caso contrário, analise todo o projeto
2. Use as ferramentas de busca para inspecionar os `using` e referências entre projetos nos arquivos `.cs` e `.csproj`
3. Verifique cada regra abaixo
4. Apresente um relatório com violações encontradas e como corrigi-las

## Mapa de dependências permitidas

```
TemplateAPI.API
  └── TemplateAPI.Servico
  └── TemplateAPI.Entidades
  └── TemplateAPI.Transversais

TemplateAPI.Servico
  └── TemplateAPI.Integracoes
  └── TemplateAPI.Entidades
  └── TemplateAPI.Transversais

TemplateAPI.Integracoes
  └── TemplateAPI.Entidades
  └── TemplateAPI.Transversais

TemplateAPI.Entidades
  └── TemplateAPI.Transversais

TemplateAPI.Transversais
  └── (sem dependências internas)
```

## Regras a verificar

### Separação de responsabilidades
- **Controllers** (`TemplateAPI.API/Controllers/`): apenas orquestração HTTP, sem lógica de negócio
- **Serviços** (`TemplateAPI.Servico/`): toda a lógica de negócio e regras
- **Integrações** (`TemplateAPI.Integracoes/`): acesso a dados externos, APIs, AWS
- **Entidades** (`TemplateAPI.Entidades/`): apenas modelos, entidades e VOs — sem lógica
- **Transversais** (`TemplateAPI.Transversais/`): utilitários compartilhados sem dependências internas

### Violações comuns a detectar
- `TemplateAPI.API` importando diretamente `TemplateAPI.Integracoes`
- `TemplateAPI.Servico` importando `TemplateAPI.API`
- Lógica de negócio dentro de controllers (queries, cálculos, regras)
- Acesso a banco/AWS direto em controllers ou serviços sem passar por `Integracoes`
- Entidades com métodos de negócio complexos (devem ser apenas data holders)
- Instanciação direta de dependências com `new` ao invés de injeção de dependência

### Verificação nos .csproj
Leia os arquivos `.csproj` de cada camada e confirme que as referências `<ProjectReference>` respeitam o mapa acima.

## Formato do relatório

```
## Verificação de Arquitetura

### Violações encontradas
- [arquivo:linha] descrição da violação e como corrigir

### Avisos (não críticos)
- [arquivo:linha] descrição

### Resultado
APROVADO / REPROVADO — resumo em uma linha
```