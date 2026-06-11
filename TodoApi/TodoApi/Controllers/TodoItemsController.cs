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

        private const string AllTodosCacheKey = "todos_all";
        private static string TodoCacheKey(long id) => $"todo_{id}";

        private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        public TodoItemsController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            if (_cache.TryGetValue(AllTodosCacheKey, out IEnumerable<TodoItemDTO>? cached))
                return Ok(cached);

            var items = await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();

            _cache.Set(AllTodosCacheKey, items, CacheOptions);

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            if (_cache.TryGetValue(TodoCacheKey(id), out TodoItemDTO? cached))
                return Ok(cached);

            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
                return NotFound();

            var dto = ItemToDTO(todoItem);
            _cache.Set(TodoCacheKey(id), dto, CacheOptions);

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO)
        {
            if (id != todoDTO.Id)
                return BadRequest();

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
                return NotFound();

            todoItem.Name = todoDTO.Name;
            todoItem.IsComplete = todoDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            _cache.Remove(TodoCacheKey(id));
            _cache.Remove(AllTodosCacheKey);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoDTO.IsComplete,
                Name = todoDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            _cache.Remove(AllTodosCacheKey);

            return CreatedAt