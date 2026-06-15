using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services.Interfaces;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoItemsController(ITodoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TodoItemDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TodoItemDTO>>> GetTodoItems(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _service.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO, CancellationToken cancellationToken)
    {
        if (id != todoDTO.Id)
            return BadRequest();

        var updated = await _service.UpdateAsync(id, todoDTO, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(todoDTO, cancellationToken);
        return CreatedAtAction(nameof(GetTodoItem), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin", Policy = "CanDelete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTodoItem(long id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
