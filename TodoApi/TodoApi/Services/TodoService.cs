using Microsoft.Extensions.Caching.Memory;
using TodoApi.Models;
using TodoApi.Repositories.Interfaces;
using TodoApi.Services.Interfaces;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TodoService> _logger;

    private static string PagedCacheKey(int page, int pageSize) => $"todos_p{page}_s{pageSize}";
    private static string TodoCacheKey(long id) => $"todo_{id}";

    private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
        .SetSlidingExpiration(TimeSpan.FromMinutes(2));

    public TodoService(ITodoRepository repository, IMemoryCache cache, ILogger<TodoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<TodoItemDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var cacheKey = PagedCacheKey(page, pageSize);

        if (_cache.TryGetValue(cacheKey, out PagedResult<TodoItemDTO>? cached))
        {
            _logger.LogDebug("Cache hit para todos página {Page}.", page);
            return cached!;
        }

        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize, cancellationToken);

        var result = new PagedResult<TodoItemDTO>
        {
            Items = items.Select(ToDTO).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        _cache.Set(cacheKey, result, CacheOptions);

        return result;
    }

    public async Task<TodoItemDTO?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TodoCacheKey(id), out TodoItemDTO? cached))
        {
            _logger.LogDebug("Cache hit para todo {Id}.", id);
            return cached;
        }

        var item = await _repository.GetByIdAsync(id, cancellationToken);

        if (item is null)
        {
            _logger.LogWarning("Todo {Id} não encontrado.", id);
            return null;
        }

        var dto = ToDTO(item);
        _cache.Set(TodoCacheKey(id), dto, CacheOptions);

        return dto;
    }

    public async Task<TodoItemDTO> CreateAsync(TodoItemDTO dto, CancellationToken cancellationToken)
    {
        var item = new TodoItem
        {
            Name = dto.Name,
            IsComplete = dto.IsComplete
        };

        var created = await _repository.CreateAsync(item, cancellationToken);

        InvalidateListCache();

        _logger.LogInformation("Todo {Id} criado.", created.Id);

        return ToDTO(created);
    }

    public async Task<bool> UpdateAsync(long id, TodoItemDTO dto, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);

        if (item is null)
        {
            _logger.LogWarning("Tentativa de atualizar todo {Id} inexistente.", id);
            return false;
        }

        item.Name = dto.Name;
        item.IsComplete = dto.IsComplete;

        var updated = await _repository.UpdateAsync(item, cancellationToken);

        if (updated)
        {
            _cache.Remove(TodoCacheKey(id));
            InvalidateListCache();
            _logger.LogInformation("Todo {Id} atualizado.", id);
        }

        return updated;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);

        if (deleted)
        {
            _cache.Remove(TodoCacheKey(id));
            InvalidateListCache();
            _logger.LogInformation("Todo {Id} deletado.", id);
        }
        else
        {
            _logger.LogWarning("Tentativa de deletar todo {Id} inexistente.", id);
        }

        return deleted;
    }

    private void InvalidateListCache()
    {
        // IMemoryCache não tem remoção por prefixo; usamos um token de cancelamento compartilhado
        // para invalidar todas as entradas de paginação de uma vez.
        if (_cache is MemoryCache mc)
            mc.Compact(1.0);
    }

    private static TodoItemDTO ToDTO(TodoItem item) => new()
    {
        Id = item.Id,
        Name = item.Name!,
        IsComplete = item.IsComplete
    };
}
