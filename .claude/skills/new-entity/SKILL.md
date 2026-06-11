---
name: new-entity
description: Cria uma nova entidade (Model) na camada TemplateAPI.Entidades seguindo os padrões do projeto. Use quando o usuário pedir para criar uma entidade, modelo ou classe de domínio.
argument-hint: <NomeDaEntidade>
---

Você irá criar uma nova entidade na camada `TemplateAPI.Entidades`.

## Entrada

O nome da entidade é: **$ARGUMENTS**

Se $ARGUMENTS não for fornecido, pergunte o nome antes de prosseguir.

## Arquivos a criar

### 1. Model — `TemplateAPI.Entidades/Models/{NomeDaEntidade}.cs`

Siga o padrão do modelo existente `TemplateAPI.Entidades/Models/Usuario.cs`:

```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TemplateAPI.Entidades.Models
{
    public class {NomeDaEntidade}
    {
        public Guid Id { get; set; }

        // Adicione as propriedades específicas da entidade aqui

        [DataType(DataType.DateTime)]
        public DateTime DataCriacao { get; set; }

        [DefaultValue(null)]
        [DataType(DataType.DateTime)]
        public DateTime? DataAlteracao { get; set; }

        [DefaultValue(null)]
        [DataType(DataType.DateTime)]
        public DateTime? DataExclusao { get; set; }
    }
}
```

## Regras

- Use `PascalCase` para nome da classe e propriedades
- Sempre inclua `Id` do tipo `Guid` como primeiro campo
- Sempre inclua as datas de auditoria (`DataCriacao`, `DataAlteracao`, `DataExclusao`)
- Propriedades opcionais devem ser `nullable` (ex: `string?`, `DateTime?`)
- Não adicione lógica de negócio na entidade — apenas propriedades
- Namespace sempre `TemplateAPI.Entidades.Models`

## Após criar

Informe o caminho do arquivo criado e sugira os próximos passos:
1. Criar a interface do serviço com `/new-feature {NomeDaEntidade}` para o fluxo completo
2. Ou criar apenas o endpoint com `/new-endpoint`
