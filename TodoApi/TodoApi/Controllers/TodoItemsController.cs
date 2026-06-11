using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TodoApi.Context;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TodoItemsController> _logger;

        private const string AllTodosCacheKey = "todos_all";
        private static string TodoCacheKey(long id) => $"todo_{id}";

        private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        public TodoItemsController(AppDbContext context, IMemoryCache cache, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoItemDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems(CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(AllTodosCacheKey, out IEnumerable<TodoItemDTO>? cached))
            {
                _logger.LogDebug("Cache hit para lista de todos.");
                return Ok(cached);
            }

            var items = await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync(cancellationToken);

            _cache.Set(AllTodosCacheKey, items, CacheOptions);

            return Ok(items);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(TodoCacheKey(id), out TodoItemDTO? cached))
            {
                _logger.LogDebug("Cache hit para todo {Id}.", id);
                return Ok(cached);
            }

            var todoItem = await _context.TodoItems.FindAsync([id], cancellationToken);

            if (todoItem == null)
            {
                _logger.LogWarning("Todo {Id} não encontrado.", id);
                return NotFound();
            }

            var dto = ItemToDTO(todoItem);
            _cache.Set(TodoCacheKey(id), dto, CacheOptions);

            return Ok(dto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO, CancellationToken cancellationToken)
        {
            if (id != todoDTO.Id)
                return BadRequest();

            var todoItem = await _context.TodoItems.FindAsync([id], cancellationToken);
            if (todoItem == null)
            {
                _logger.LogWarning("Tentativa de atualizar todo {Id} inexistente.", id);
                return NotFound();
            }

            todoItem.Name = todoDTO.Name;
            todoItem.IsComplete = todoDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            _cache.Remove(TodoCacheKey(id));
            _cache.Remove(AllTodosCacheKey);

            _logger.LogInformation("Todo {Id} atualizado.", id);

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoItemDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO, CancellationToken cancellationToken)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoDTO.IsComplete,
                Name = todoDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync(cancellationToken);

            _cache.Remove(AllTodosCacheKey);

            _logger.LogInformation("Todo {Id} criado.", todoItem.Id);

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.Id },
                ItemToDTO(todoItem));
        }

        [Authorize(Roles = "Admin", Policy = "CanDelete")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoItem(long id, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync([id], cancellationToken);
            if (todoItem == null)
            {
                _logger.LogWarning("Tentativa de deletar todo {Id} inexistente.", id);
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync(cancellationToken);

            _cache.Remove(TodoCacheKey(id));
            _cache.Remove(AllTodosCacheKey);

            _logger.LogInformation("Todo {Id} deletado.", id);

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
