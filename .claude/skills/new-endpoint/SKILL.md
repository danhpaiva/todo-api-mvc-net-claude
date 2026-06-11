---
name: new-endpoint
description: Adiciona um novo endpoint a um controller existente seguindo os padrões do TodoApi. Use quando o usuário pedir para criar um endpoint, rota ou action em um controller já existente.
argument-hint: <NomeDoController> <Verbo> <NomeDoEndpoint>
---

Você irá adicionar um novo endpoint a um controller existente.

## Entrada

Os argumentos são: **$ARGUMENTS**

Extraia:
- **Controller**: qual controller receberá o endpoint (ex: `TodoItems`)
- **Verbo HTTP**: `GET`, `POST`, `PUT`, `DELETE`, `PATCH`
- **Nome da ação**: o que ele faz (ex: `GetByName`, `MarkComplete`)

Se alguma informação estiver faltando, pergunte antes de prosseguir.

## Passos

1. Leia o controller alvo em `TodoApi/Controllers/{Controller}Controller.cs`
2. Leia o model/DTO correspondente em `TodoApi/Models/`
3. Gere o método seguindo o padrão abaixo
4. Se o endpoint for de leitura (GET), adicione lógica de cache
5. Se o endpoint for de escrita (POST/PUT/DELETE), adicione invalidação de cache

## Padrão do endpoint

Baseie-se no `TodoItemsController.cs` existente:

```csharp
[Http{Verbo}("{rota}")]
[ProducesResponseType(typeof({TipoDeRetorno}), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<{TipoDeRetorno}>> {NomeDaAcao}({parametros})
{
    // Para GET: verificar cache antes de buscar no banco
    // if (_cache.TryGetValue(CacheKey, out {Tipo}? cached))
    //     return Ok(cached);

    var item = await _context.{DbSet}.{consulta};

    if (item == null)
        return NotFound();

    // Para GET: armazenar no cache
    // _cache.Set(CacheKey, item, CacheOptions);

    return Ok(item);
}
```

## Regras

- Sempre use `async/await`
- Adicione `[ProducesResponseType]` para cada status HTTP possível
- Para `POST`/`PUT`: receba o body via `[FromBody]`
- Para `GET`/`DELETE`: receba parâmetros via `[FromQuery]` ou route param
- Endpoints GET devem usar cache (`TryGetValue` → banco → `Set`)
- Endpoints de escrita devem invalidar o cache com `_cache.Remove(...)`
- Nunca exponha a entidade EF diretamente — use o DTO e o método `ItemToDTO`

## Após criar

Liste o arquivo modificado e o endpoint gerado com método HTTP e rota.
