using Microsoft.EntityFrameworkCore;
using TodoApi.Context;
using TodoApi.Models;
using TodoApi.Repositories.Interfaces;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
        => await _context.TodoItems.ToListAsync(cancellationToken);

    public async Task<TodoItem?> GetByIdAsync(long id, CancellationToken cancellationToken)
        => await _context.TodoItems.FindAsync([id], cancellationToken);

    public async Task<TodoItem> CreateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ExistsAsync(item.Id, cancellationToken))
                return false;
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var item = await _context.TodoItems.FindAsync([id], cancellationToken);
        if (item is null) return false;

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken)
        => await _context.TodoItems.AnyAsync(e => e.Id == id, cancellationToken);
}
