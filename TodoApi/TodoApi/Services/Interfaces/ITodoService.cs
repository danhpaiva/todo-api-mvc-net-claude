using TodoApi.Models;

namespace TodoApi.Services.Interfaces;

public interface ITodoService
{
    Task<PagedResult<TodoItemDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<TodoItemDTO?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<TodoItemDTO> CreateAsync(TodoItemDTO dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, TodoItemDTO dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
