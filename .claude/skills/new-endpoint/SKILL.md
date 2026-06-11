---
name: new-endpoint
description: Adiciona um novo endpoint a um controller existente seguindo os padrões do projeto. Use quando o usuário pedir para criar um endpoint, rota ou action em um controller já existente.
argument-hint: <NomeDoController> <Verbo> <NomeDoEndpoint>
---

Você irá adicionar um novo endpoint a um controller existente.

## Entrada

Os argumentos são: **$ARGUMENTS**

Extraia do argumento:
- **Controller**: qual controller receberá o endpoint (ex: `Usuario`, `Pdf`)
- **Verbo HTTP**: `GET`, `POST`, `PUT`, `DELETE`, `PATCH`
- **Nome do endpoint/ação**: o que ele faz (ex: `ObterPorEmail`, `Atualizar`, `Remover`)

Se alguma informação estiver faltando, pergunte antes de prosseguir.

## Passos

1. Leia o controller alvo em `TemplateAPI.API/Controllers/{Controller}Controller.cs`
2. Leia a interface do serviço correspondente em `TemplateAPI.Servico/Interface/I{Controller}Servico.cs`
3. Gere o método no controller seguindo o padrão abaixo
4. Adicione a assinatura do método na interface do serviço
5. Adicione a implementação do método em `TemplateAPI.Servico/Implementacao/{Controller}Servico.cs`

## Padrão do endpoint no controller

Baseie-se no padrão do `UsuarioController`:

```csharp
/// <summary>
/// {Descrição do endpoint}
/// </summary>
[Http{Verbo}("{rota}")]
[ProducesResponseType(typeof({TipoDeRetorno}), (int)HttpStatusCode.OK)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
public async Task<ActionResult<{TipoDeRetorno}>> {NomeDoEndpoint}Async({parametros}, CancellationToken cancellationToken)
{
    try
    {
        var retorno = await _{servicoPrivado}.{NomeDoEndpoint}({parametros}, cancellationToken);
        return Ok(retorno);
    }
    catch (Exception ex)
    {
        return ExceptionReturn(ex, "Erro ao {descrição}");
    }
}
```

## Regras

- Sempre use `async/await` e receba `CancellationToken cancellationToken`
- Sempre envolva em `try/catch` usando `ExceptionReturn`
- Nomes de métodos em `PascalCase` com sufixo `Async`
- Adicione `[ProducesResponseType]` para cada status HTTP possível
- Não coloque lógica de negócio no controller — apenas delegue para o serviço
- Para `POST`/`PUT`, receba o body via `[FromBody]`
- Para `GET`/`DELETE`, receba parâmetros via `[FromQuery]` ou `[FromRoute]`

## Após criar

Liste todos os arquivos modificados e o endpoint gerado com método e rota.
