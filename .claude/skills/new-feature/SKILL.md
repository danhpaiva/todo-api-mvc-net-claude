---
name: new-feature
description: Cria o esqueleto completo de uma nova feature no TodoApi: entidade, DTO, DbSet e controller com CRUD básico. Use quando o usuário pedir para criar uma nova funcionalidade do zero.
argument-hint: <NomeDaFeature>
---

Você irá criar todos os arquivos necessários para uma nova feature completa.

## Entrada

O nome da feature é: **$ARGUMENTS**

Se $ARGUMENTS não for fornecido, pergunte o nome antes de prosseguir.

Use o nome para derivar:
- **NomeDaEntidade**: PascalCase singular (ex: `Categoria`)
- **NomeDoController**: `{NomeDaEntidade}sController` (ex: `CategoriasController`)
- **nomePlural**: plural para rota e DbSet (ex: `categorias`, `Categorias`)

---

## Passo 1 — Criar entidade e DTO

Execute todos os passos da skill **new-entity** para `{NomeDaEntidade}`.

Isso criará:
- `TodoApi/Models/{NomeDaEntidade}.cs`
- `TodoApi/Models/{NomeDaEntidade}DTO.cs`
- DbSet em `TodoApi/Context/AppDbContext.cs`

---

## Passo 2 — Criar o controller com CRUD

Crie `TodoApi/Controllers/{NomeDoController}.cs` seguindo o padrão de `TodoItemsController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TodoApi.Context;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class {NomeDoController} : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        private const string AllCacheKey = "{nomePlural}_all";
        private static string ItemCacheKey(long id) => $"{nomePlural}_{id}";

        private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        public {NomeDoController}(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<{NomeDaEntidade}DTO>>> GetAll()
        {
            if (_cache.TryGetValue(AllCacheKey, out IEnumerable<{NomeDaEntidade}DTO>? cached))
                return Ok(cached);

            var items = await _context.{DbSet}.Select(x => ItemToDTO(x)).ToListAsync();
            _cache.Set(AllCacheKey, items, CacheOptions);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<{NomeDaEntidade}DTO>> GetById(long id)
        {
            if (_cache.TryGetValue(ItemCacheKey(id), out {NomeDaEntidade}DTO? cached))
                return Ok(cached);

            var item = await _context.{DbSet}.FindAsync(id);
            if (item == null) return NotFound();

            var dto = ItemToDTO(item);
            _cache.Set(ItemCacheKey(id), dto, CacheOptions);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<{NomeDaEntidade}DTO>> Create({NomeDaEntidade}DTO dto)
        {
            var item = new {NomeDaEntidade} { /* mapear propriedades */ };
            _context.{DbSet}.Add(item);
            await _context.SaveChangesAsync();
            _cache.Remove(AllCacheKey);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, ItemToDTO(item));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, {NomeDaEntidade}DTO dto)
        {
            if (id != dto.Id) return BadRequest();
            var item = await _context.{DbSet}.FindAsync(id);
            if (item == null) return NotFound();

            // mapear propriedades do dto para item

            await _context.SaveChangesAsync();
            _cache.Remove(ItemCacheKey(id));
            _cache.Remove(AllCacheKey);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.{DbSet}.FindAsync(id);
            if (item == null) return NotFound();

            _context.{DbSet}.Remove(item);
            await _context.SaveChangesAsync();
            _cache.Remove(ItemCacheKey(id));
            _cache.Remove(AllCacheKey);
            return NoContent();
        }

        private static {NomeDaEntidade}DTO ItemToDTO({NomeDaEntidade} item) =>
            new {NomeDaEntidade}DTO { Id = item.Id /* mapear demais propriedades */ };
    }
}
```

---

## Passo 3 — Gerar os testes

Execute todos os passos da skill **generate-tests** para `{NomeDaFeature}`.

---

## Regras

- Sempre use `IMemoryCache` no controller
- Nunca exponha entidades EF diretamente — use DTOs
- Invalide o cache em toda operação de escrita
- Preencha os mapeamentos `ItemToDTO` e `Create`/`Update` com as propriedades reais da entidade

## Ao concluir

Liste todos os arquivos criados/modificados e lembre o desenvolvedor de:
1. Adicionar as propriedades reais na entidade e no DTO
2. Preencher os mapeamentos no controller marcados com comentários
