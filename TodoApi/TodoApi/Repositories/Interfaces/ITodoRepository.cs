using TodoApi.Models;

namespace TodoApi.Repositories.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoItem?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<TodoItem> CreateAsync(TodoItem item, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(TodoItem item, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
}
