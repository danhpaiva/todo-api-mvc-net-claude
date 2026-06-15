using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TodoApi.Context;
using TodoApi.Models;
using TodoApi.Services.Interfaces;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TodoService> _logger;

    private const string AllTodosCacheKey = "todos_all";
    private static string TodoCacheKey(long id) => $"todo_{id}";

    private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
        .SetSlidingExpiration(TimeSpan.FromMinutes(2));

    public TodoService(AppDbContext context, IMemoryCache cache, ILogger<TodoService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<TodoItemDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(AllTodosCacheKey, out IEnumerable<TodoItemDTO>? cached))
        {
            _logger.LogDebug("Cache hit para lista de todos.");
            return cached!;
        }

        var items = await _context.TodoItems
            .Select(x => ToDTO(x))
            .ToListAsync(cancellationToken);

        _cache.Set(AllTodosCacheKey, items, CacheOptions);

        return items;
    }

    public async Task<TodoItemDTO?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TodoCacheKey(id), out TodoItemDTO? cached))
        {
            _logger.LogDebug("Cache hit para todo {Id}.", id);
            return cached;
        }

        var item = await _context.TodoItems.FindAsync([id], cancellationToken);

        if (item == null)
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

        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        _cache.Remove(AllTodosCacheKey);

        _logger.LogInformation("Todo {Id} criado.", item.Id);

        return ToDTO(item);
    }

    public async Task<bool> UpdateAsync(long id, TodoItemDTO dto, CancellationToken cancellationToken)
    {
        var item = await _context.TodoItems.FindAsync([id], cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Tentativa de atualizar todo {Id} inexistente.", id);
            return false;
        }

        item.Name = dto.Name;
        item.IsComplete = dto.IsComplete;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException) when (!_context.TodoItems.Any(e => e.Id == id))
        {
            return false;
        }

        _cache.Remove(TodoCacheKey(id));
        _cache.Remove(AllTodosCacheKey);

        _logger.LogInformation("Todo {Id} atualizado.", id);

        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var item = await _context.TodoItems.FindAsync([id], cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Tentativa de deletar todo {Id} inexistente.", id);
            return false;
        }

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        _cache.Remove(TodoCacheKey(id));
        _cache.Remove(AllTodosCacheKey);

        _logger.LogInformation("Todo {Id} deletado.", id);

        return true;
    }

    private static TodoItemDTO ToDTO(TodoItem item) => new()
    {
        Id = item.Id,
        Name = item.Name!,
        IsComplete = item.IsComplete
    };
}
